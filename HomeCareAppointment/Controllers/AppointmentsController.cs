using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;

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
        // (Beholder din nåværende visning som viser AvailableDays med status)
        public async Task<IActionResult> Index()
        {
            var days = await _days.GetAllWithRelationsAsync();
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

            // Gi viewet en modell-instans slik at tag helpers fungerer
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

            // Avledede felt
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
            var appt = await _appointments.GetByIdAsync(id.Value);
            if (appt is null) return NotFound();

            var days = await _days.GetAllAsync();
            var pats = await _patients.GetAllAsync();
            var pers = await _personnels.GetAllAsync();

            ViewData["AvailableDayId"] = new SelectList(days, "Id", "Id", appt.AvailableDayId);
            ViewData["PatientId"]      = new SelectList(pats, "PatientId", "Name", appt.PatientId);
            ViewData["PersonnelId"]    = new SelectList(pers, "Id", "Id", appt.PersonnelId);

            return View(appt);
        }

        // POST: Appointments/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Date,Notes,PatientId,PersonnelId,AvailableDayId")] Appointment appointment)
        {
            if (id != appointment.AppointmentId) return NotFound();

            if (!ModelState.IsValid)
            {
                var days = await _days.GetAllAsync();
                var pats = await _patients.GetAllAsync();
                var pers = await _personnels.GetAllAsync();

                ViewData["AvailableDayId"] = new SelectList(days, "Id", "Id", appointment.AvailableDayId);
                ViewData["PatientId"]      = new SelectList(pats, "PatientId", "Name", appointment.PatientId);
                ViewData["PersonnelId"]    = new SelectList(pers, "Id", "Id", appointment.PersonnelId);

                return View(appointment);
            }

            await _appointments.UpdateAsync(appointment);
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
    }
}