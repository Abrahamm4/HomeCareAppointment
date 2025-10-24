using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AvailableDayDbContext _context;

        public AppointmentsController(AvailableDayDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var days = await _context.AvailableDays
                .Include(d => d.Personnel)
                .Include(d => d.Appointment)
                .ToListAsync();

            // Load all patients for dropdown (temporary until login)
            ViewBag.Patients = await _context.Patients
                .ToListAsync();


            return View(days);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.AvailableDay)
                .Include(a => a.Patient)
                .Include(a => a.Personnel)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create(int availableDayId)
        {
            var day = _context.AvailableDays
                .Include(d => d.Personnel)
                .Include(d => d.Appointment)
                .FirstOrDefault(d => d.Id == availableDayId);

            if (day == null) return NotFound();

            if (day.Appointment != null)
            {
                // Slot already booked
                return BadRequest("Selected slot is already booked.");
            }

            ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
            ViewBag.AvailableDayId = day.Id;

            // Temporary patient dropdown until authentication is added
            ViewBag.PatientId = new SelectList(_context.Patients, "PatientId", "Name");

            // Provide an Appointment model instance so the view's tag helpers can work
            var model = new Appointment
            {
                AvailableDayId = day.Id,
                Date = day.Date
            };

            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,AvailableDayId,Notes")] Appointment appointment)
        {
            if (appointment == null) return BadRequest();

            // Get the chosen AvailableDay and its personnel
            var day = await _context.AvailableDays
                .Include(d => d.Personnel)
                .Include(d => d.Appointment)
                .FirstOrDefaultAsync(d => d.Id == appointment.AvailableDayId);

            if (day == null) return NotFound();

            if (day.Appointment != null)
            {
                ModelState.AddModelError(string.Empty, "This slot is already booked.");
                ViewBag.PatientId = new SelectList(_context.Patients, "PatientId", "Name", appointment.PatientId);
                ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                return View(appointment);
            }

            // Set derived values that are not posted from the form
            appointment.PersonnelId = day.PersonnelId;
            appointment.Date = day.Date;

            if (!ModelState.IsValid)
            {
                ViewBag.PatientId = new SelectList(_context.Patients, "PatientId", "Name", appointment.PatientId);
                ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                return View(appointment);
            }

            _context.Add(appointment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.AvailableDay)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Prepare list of available days: those without an appointment or the current one
            var days = await _context.AvailableDays
                .Include(d => d.Personnel)
                .Include(d => d.Appointment)
                .Where(d => d.Appointment == null || d.Id == appointment.AvailableDayId)
                .ToListAsync();

            var selectItems = days.Select(d => new {
                d.Id,
                Text = $"{d.Personnel?.Name} - {d.Date:dd MMM yyyy} {d.StartTime:hh\\:mm}-{d.EndTime:hh\\:mm}"
            }).ToList();

            ViewBag.AvailableDayId = new SelectList(selectItems, "Id", "Text", appointment.AvailableDayId);

            // Keep patient info for display (not editable)
            ViewBag.PatientName = appointment.Patient?.Name;

            // Keep original available day id for client-side change notice
            ViewBag.OriginalAvailableDayId = appointment.AvailableDayId;

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Notes,AvailableDayId")] Appointment appointment)
        {
            if (appointment == null) return BadRequest();
            if (id != appointment.AppointmentId) return NotFound();

            // Load original appointment
            var original = await _context.Appointments
                .Include(a => a.AvailableDay)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (original == null) return NotFound();

            // If AvailableDay changed, move booking to new AvailableDay (create new appointment) and remove old
            if (appointment.AvailableDayId != original.AvailableDayId)
            {
                var newDay = await _context.AvailableDays
                    .Include(d => d.Personnel)
                    .Include(d => d.Appointment)
                    .FirstOrDefaultAsync(d => d.Id == appointment.AvailableDayId);

                if (newDay == null) return NotFound();

                if (newDay.Appointment != null)
                {
                    // Slot taken
                    ModelState.AddModelError(string.Empty, "Selected slot is already booked.");

                    // Rebuild select list and return
                    var days = await _context.AvailableDays
                        .Include(d => d.Personnel)
                        .Include(d => d.Appointment)
                        .Where(d => d.Appointment == null || d.Id == original.AvailableDayId)
                        .ToListAsync();

                    var selectItems = days.Select(d => new {
                        d.Id,
                        Text = $"{d.Personnel?.Name} - {d.Date:dd MMM yyyy} {d.StartTime:hh\\:mm}-{d.EndTime:hh\\:mm}"
                    }).ToList();

                    ViewBag.AvailableDayId = new SelectList(selectItems, "Id", "Text", appointment.AvailableDayId);
                    ViewBag.PatientName = original.Patient?.Name;
                    // ensure original id is available to view
                    ViewBag.OriginalAvailableDayId = original.AvailableDayId;
                    return View(original);
                }

                // Create new appointment for the selected day using patient from original
                var newAppointment = new Appointment
                {
                    PatientId = original.PatientId,
                    PersonnelId = newDay.PersonnelId,
                    AvailableDayId = newDay.Id,
                    Date = newDay.Date,
                    Notes = appointment.Notes
                };

                _context.Appointments.Add(newAppointment);

                // Remove original appointment
                _context.Appointments.Remove(original);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Same AvailableDay: just update notes
            original.Notes = appointment.Notes;

            try
            {
                _context.Update(original);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(original.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.AvailableDay)
                .Include(a => a.Patient)
                .Include(a => a.Personnel)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Appointments/ManageAdmin
        public async Task<IActionResult> ManageAdmin()
        {
            var appointments = await _context.Appointments
                .Include(a => a.AvailableDay)
                .Include(a => a.Personnel)
                .Include(a => a.Patient)
                .OrderBy(a => a.AvailableDay.Date)
                .ToListAsync();

            // ViewData["PatientMode"] = false;
            return View("Manage", appointments);
        }

        // POST: Appointments/ManagePatient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePatient(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);

            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.AvailableDay)
                .Include(a => a.Personnel)
                .Include(a => a.Patient)
                .OrderBy(a => a.AvailableDay.Date)
                .ToListAsync();

            ViewData["PatientMode"] = true;
            ViewData["SelectedPatient"] = patient;

            return View("Manage", appointments);
        }

        // GET: Appointments/ManagePatient
        [HttpGet]
        public async Task<IActionResult> ManagePatient(int? patientId)
        {
            if (patientId == null)
            {
                return BadRequest();
            }

            var patient = await _context.Patients.FindAsync(patientId.Value);

            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId.Value)
                .Include(a => a.AvailableDay)
                .Include(a => a.Personnel)
                .Include(a => a.Patient)
                .OrderBy(a => a.AvailableDay.Date)
                .ToListAsync();

            ViewData["PatientMode"] = true;
            ViewData["SelectedPatient"] = patient;

            return View("Manage", appointments);
        }



        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
