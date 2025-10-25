using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;
using System.Collections.Generic;

namespace HomeCareAppointment.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointments;
        private readonly IAvailableDayRepository _days;
        private readonly IPatientRepository _patients;
        private readonly IPersonnelRepository _personnels;

        public AppointmentsController(
            IAppointmentRepository appointments,
            IAvailableDayRepository days,
            IPatientRepository patients,
            IPersonnelRepository personnels)
        {
            _appointments = appointments;
            _days = days;
            _patients = patients;
            _personnels = personnels;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var days = await _days.GetAllWithRelationsAsync();

            // Load all patients for dropdown (temporary until login)
            var patients = await _patients.GetAllAsync();
            ViewBag.Patients = patients;

            return View(days);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            var appt = await _appointments.GetByIdWithRelationsAsync(id.Value);
            if (appt is null) return NotFound();
            return View(appt);
        }

        // GET: Appointments/Create?availableDayId=#
        public async Task<IActionResult> Create(int availableDayId)
        {
            var day = await _days.GetByIdWithRelationsAsync(availableDayId);
            if (day is null) return NotFound();

            if (day.Appointment != null)
            {
                // Slot already booked
                return BadRequest("Selected slot is already booked.");
            }

            ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
            ViewBag.AvailableDayId = day.Id;

            var allPatients = await _patients.GetAllAsync();
            ViewBag.PatientId = new SelectList(allPatients, "PatientId", "Name");

            var model = new Appointment
            {
                AvailableDayId = day.Id,
                Date = day.Date
            };
            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,AvailableDayId,Notes")] Appointment appointment)
        {
            if (appointment is null) return BadRequest();

            var day = await _days.GetByIdWithRelationsAsync(appointment.AvailableDayId);
            if (day is null) return NotFound();

            if (day.Appointment != null)
            {
                ModelState.AddModelError(string.Empty, "This slot is already booked.");
                var allPatients = await _patients.GetAllAsync();
                ViewBag.PatientId = new SelectList(allPatients, "PatientId", "Name", appointment.PatientId);
                ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                return View(appointment);
            }

            // Derived fields
            appointment.PersonnelId = day.PersonnelId;
            appointment.Date = day.Date;

            if (!ModelState.IsValid)
            {
                var allPatients = await _patients.GetAllAsync();
                ViewBag.PatientId = new SelectList(allPatients, "PatientId", "Name", appointment.PatientId);
                ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                return View(appointment);
            }

            await _appointments.CreateAsync(appointment);
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();
            var appt = await _appointments.GetByIdWithRelationsAsync(id.Value);
            if (appt is null) return NotFound();

            // Prepare list of available days: those without an appointment or the current one
            var days = (await _days.GetAllWithRelationsAsync())
                .Where(d => d.Appointment == null || d.Id == appt.AvailableDayId)
                .ToList();

            var selectItems = days.Select(d => new {
                d.Id,
                Text = $"{d.Personnel?.Name} - {d.Date:dd MMM yyyy} {d.StartTime:hh\\:mm}-{d.EndTime:hh\\:mm}"
            }).ToList();

            ViewBag.AvailableDayId = new SelectList(selectItems, "Id", "Text", appt.AvailableDayId);

            // Keep patient info for display (not editable)
            ViewBag.PatientName = appt.Patient?.Name;

            // Keep original available day id for client-side change notice
            ViewBag.OriginalAvailableDayId = appt.AvailableDayId;

            return View(appt);
        }

        // POST: Appointments/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Notes,AvailableDayId")] Appointment appointment)
        {
            if (appointment == null) return BadRequest();
            if (id != appointment.AppointmentId) return NotFound();

            // Load original appointment
            var original = await _appointments.GetByIdWithRelationsAsync(id);
            if (original == null) return NotFound();

            // If AvailableDay changed, move booking to new AvailableDay (create new appointment) and remove old
            if (appointment.AvailableDayId != original.AvailableDayId)
            {
                var newDay = await _days.GetByIdWithRelationsAsync(appointment.AvailableDayId);
                if (newDay == null) return NotFound();

                if (newDay.Appointment != null)
                {
                    // Slot taken
                    ModelState.AddModelError(string.Empty, "Selected slot is already booked.");

                    // Rebuild select list and return
                    var days = (await _days.GetAllWithRelationsAsync())
                        .Where(d => d.Appointment == null || d.Id == original.AvailableDayId)
                        .ToList();

                    var selectItems = days.Select(d => new {
                        d.Id,
                        Text = $"{d.Personnel?.Name} - {d.Date:dd MMM yyyy} {d.StartTime:hh\\:mm}-{d.EndTime:hh\\:mm}"
                    }).ToList();

                    ViewBag.AvailableDayId = new SelectList(selectItems, "Id", "Text", appointment.AvailableDayId);
                    ViewBag.PatientName = original.Patient?.Name;
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

                await _appointments.CreateAsync(newAppointment);

                // Remove original appointment
                await _appointments.DeleteAsync(original.AppointmentId);

                return RedirectToAction(nameof(Index));
            }

            // Same AvailableDay: just update notes
            original.Notes = appointment.Notes;

            await _appointments.UpdateAsync(original);

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var appt = await _appointments.GetByIdWithRelationsAsync(id.Value);
            if (appt is null) return NotFound();
            return View(appt);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _appointments.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/ManageAdmin
        public async Task<IActionResult> ManageAdmin()
        {
            var appointments = await _appointments.GetAllWithRelationsAsync();
            var ordered = appointments.OrderBy(a => a.AvailableDay?.Date).ToList();

            return View("Manage", ordered);
        }

        // POST: Appointments/ManagePatient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePatient(int patientId)
        {
            var patient = await _patients.GetByIdAsync(patientId);

            var appointments = (await _appointments.GetAllWithRelationsAsync())
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AvailableDay?.Date)
                .ToList();

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

            var patient = await _patients.GetByIdAsync(patientId.Value);

            var appointments = (await _appointments.GetAllWithRelationsAsync())
                .Where(a => a.PatientId == patientId.Value)
                .OrderBy(a => a.AvailableDay?.Date)
                .ToList();

            ViewData["PatientMode"] = true;
            ViewData["SelectedPatient"] = patient;

            return View("Manage", appointments);
        }

        private async Task<bool> AppointmentExists(int id)
        {
            var a = await _appointments.GetByIdAsync(id);
            return a != null;
        }
    }
}