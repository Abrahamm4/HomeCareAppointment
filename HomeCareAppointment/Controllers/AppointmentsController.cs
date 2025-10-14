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
            var unbookedDays = await _context.AvailableDays
                .Include(d => d.Personnel)
                .Include(d => d.Appointments)
                .Where(d => !d.Appointments.Any()) // only free slots
                .ToListAsync();

            return View(unbookedDays);
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
                .FirstOrDefault(d => d.Id == availableDayId);

            if (day == null) return NotFound();

            ViewBag.AvailableDayInfo = $"{day.Personnel.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
            ViewBag.AvailableDayId = day.Id;

            // Temporary patient dropdown until authentication is added
            //ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName");

            return View();
        }


        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create([Bind("PatientId,AvailableDayId,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Get personnel from the chosen AvailableDay
                var day = await _context.AvailableDays
                    .Include(d => d.Personnel)
                    .FirstOrDefaultAsync(d => d.Id == appointment.AvailableDayId);

                if (day == null) return NotFound();

                appointment.PersonnelId = day.PersonnelId;
              //  appointment.BookedAt = DateTime.Now;

                _context.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["AvailableDayId"] = new SelectList(_context.AvailableDays, "Id", "Id", appointment.AvailableDayId);
            ViewData["PatientId"] = new SelectList(_context.Set<Patient>(), "PatientId", "Name", appointment.PatientId);
            ViewData["PersonnelId"] = new SelectList(_context.Personnels, "Id", "Id", appointment.PersonnelId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Date,Notes,PatientId,PersonnelId,AvailableDayId")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["AvailableDayId"] = new SelectList(_context.AvailableDays, "Id", "Id", appointment.AvailableDayId);
            ViewData["PatientId"] = new SelectList(_context.Set<Patient>(), "PatientId", "Name", appointment.PatientId);
            ViewData["PersonnelId"] = new SelectList(_context.Personnels, "Id", "Id", appointment.PersonnelId);
            return View(appointment);
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

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
