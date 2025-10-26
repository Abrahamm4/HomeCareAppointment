using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HomeCareAppointment.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointments;
        private readonly IAvailableDayRepository _days;
        private readonly IPatientRepository _patients;
        private readonly IPersonnelRepository _personnels;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentRepository appointments,
            IAvailableDayRepository days,
            IPatientRepository patients,
            IPersonnelRepository personnels,
            ILogger<AppointmentsController> logger)
        {
            _appointments = appointments;
            _days = days;
            _patients = patients;
            _personnels = personnels;
            _logger = logger;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            try
            {
                var days = await _days.GetAllWithRelationsAsync() ?? Enumerable.Empty<AvailableDay>();
                var patients = await _patients.GetAllAsync() ?? Enumerable.Empty<Patient>();

                ViewBag.Patients = patients;
                return View(days);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Index failed");
                return View("Error", new { message = "Failed to load appointments." });
            }
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            try
            {
                var appt = await _appointments.GetByIdWithRelationsAsync(id.Value);
                if (appt is null) return NotFound();
                return View(appt);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Details failed for Id {Id}", id);
                return BadRequest();
            }
        }

        // GET: Appointments/Create?availableDayId=#
        public async Task<IActionResult> Create(int availableDayId)
        {
            try
            {
                var day = await _days.GetByIdWithRelationsAsync(availableDayId);
                if (day is null) return NotFound();

                if (day.Appointment != null) return BadRequest("Selected slot is already booked.");

                ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                ViewBag.AvailableDayId = day.Id;

                var allPatients = await _patients.GetAllAsync() ?? Enumerable.Empty<Patient>();
                ViewBag.PatientId = new SelectList(allPatients, "PatientId", "Name");

                var model = new Appointment
                {
                    AvailableDayId = day.Id,
                    Date = day.Date
                };
                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Create(GET) failed for AvailableDayId {Id}", availableDayId);
                return BadRequest();
            }
        }

        // POST: Appointments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,AvailableDayId,Notes")] Appointment appointment)
        {
            if (appointment == null) return BadRequest();

            try
            {
                var day = await _days.GetByIdWithRelationsAsync(appointment.AvailableDayId);
                if (day == null) return NotFound();

                if (day.Appointment != null)
                {
                    ModelState.AddModelError(string.Empty, "This slot is already booked.");
                    var allPatients = await _patients.GetAllAsync() ?? Enumerable.Empty<Patient>();
                    ViewBag.PatientId = new SelectList(allPatients, "PatientId", "Name", appointment.PatientId);
                    ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                    return View(appointment);
                }

                appointment.PersonnelId = day.PersonnelId;
                appointment.Date = day.Date;

                if (!ModelState.IsValid)
                {
                    var allPatients = await _patients.GetAllAsync() ?? Enumerable.Empty<Patient>();
                    ViewBag.PatientId = new SelectList(allPatients, "PatientId", "Name", appointment.PatientId);
                    ViewBag.AvailableDayInfo = $"{day.Personnel?.Name} - {day.Date:dd MMM yyyy} {day.StartTime}-{day.EndTime}";
                    return View(appointment);
                }

                var success = await _appointments.CreateAsync(appointment);
                if (!success) _logger.LogError("[AppointmentsController] Create failed for appointment {@Appointment}", appointment);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Create(POST) failed");
                return BadRequest();
            }
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            try
            {
                var appt = await _appointments.GetByIdWithRelationsAsync(id.Value);
                if (appt == null) return NotFound();

                var allDays = await _days.GetAllWithRelationsAsync() ?? Enumerable.Empty<AvailableDay>();
                var days = allDays.Where(d => d.Appointment == null || d.Id == appt.AvailableDayId).ToList();

                var selectItems = days.Select(d => new {
                    d.Id,
                    Text = $"{d.Personnel?.Name} - {d.Date:dd MMM yyyy} {d.StartTime:hh\\:mm}-{d.EndTime:hh\\:mm}"
                }).ToList();

                ViewBag.AvailableDayId = new SelectList(selectItems, "Id", "Text", appt.AvailableDayId);
                ViewBag.PatientName = appt.Patient?.Name;
                ViewBag.OriginalAvailableDayId = appt.AvailableDayId;

                return View(appt);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Edit(GET) failed for Id {Id}", id);
                return BadRequest();
            }
        }

        // POST: Appointments/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Notes,AvailableDayId")] Appointment appointment)
        {
            if (appointment == null || id != appointment.AppointmentId) return BadRequest();

            try
            {
                var original = await _appointments.GetByIdWithRelationsAsync(id);
                if (original == null) return NotFound();

                if (appointment.AvailableDayId != original.AvailableDayId)
                {
                    var newDay = await _days.GetByIdWithRelationsAsync(appointment.AvailableDayId);
                    if (newDay == null) return NotFound();

                    if (newDay.Appointment != null)
                    {
                        ModelState.AddModelError(string.Empty, "Selected slot is already booked.");
                        return View(original);
                    }

                    var newAppointment = new Appointment
                    {
                        PatientId = original.PatientId,
                        PersonnelId = newDay.PersonnelId,
                        AvailableDayId = newDay.Id,
                        Date = newDay.Date,
                        Notes = appointment.Notes
                    };

                    var created = await _appointments.CreateAsync(newAppointment);
                    if (!created) _logger.LogError("[AppointmentsController] Edit failed creating new appointment {@Appointment}", newAppointment);

                    var deleted = await _appointments.DeleteAsync(original.AppointmentId);
                    if (!deleted) _logger.LogError("[AppointmentsController] Edit failed deleting original appointment {@Appointment}", original);

                    return RedirectToAction(nameof(Index));
                }

                original.Notes = appointment.Notes;
                var updated = await _appointments.UpdateAsync(original);
                if (!updated) _logger.LogError("[AppointmentsController] Edit failed updating appointment {@Appointment}", original);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Edit(POST) failed for Id {Id}", id);
                return BadRequest();
            }
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            try
            {
                var appt = await _appointments.GetByIdWithRelationsAsync(id.Value);
                if (appt == null) return NotFound();
                return View(appt);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] Delete(GET) failed for Id {Id}", id);
                return BadRequest();
            }
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _appointments.DeleteAsync(id);
                if (!success) _logger.LogWarning("[AppointmentsController] DeleteConfirmed: Appointment not found for Id {Id}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] DeleteConfirmed failed for Id {Id}", id);
                return BadRequest();
            }
        }
        // GET: Appointments/ManageAdmin
        public async Task<IActionResult> ManageAdmin()
        {
            try
            {
                var appointments = await _appointments.GetAllWithRelationsAsync() ?? Enumerable.Empty<Appointment>();
                var ordered = appointments.OrderBy(a => a.AvailableDay?.Date).ToList();

                return View("Manage", ordered);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] ManageAdmin failed");
                return View("Error", new { message = "Failed to load admin appointment management." });
            }
}

        // POST: Appointments/ManagePatient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePatient(int patientId)
        {
            try
            {
                var patient = await _patients.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("[AppointmentsController] ManagePatient POST - No patient found for ID {PatientId}", patientId);
                    return NotFound();
                }

                var appointments = (await _appointments.GetAllWithRelationsAsync() ?? Enumerable.Empty<Appointment>())
                    .Where(a => a.PatientId == patientId)
                    .OrderBy(a => a.AvailableDay?.Date)
                    .ToList();

                ViewData["PatientMode"] = true;
                ViewData["SelectedPatient"] = patient;

                return View("Manage", appointments);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] ManagePatient POST failed for patientId {PatientId}", patientId);
                return View("Error", new { message = "Failed to load patient appointments." });
            }
        }

        // GET: Appointments/ManagePatient
        [HttpGet]
        public async Task<IActionResult> ManagePatient(int? patientId)
        {
            if (patientId == null)
            {
                _logger.LogWarning("[AppointmentsController] ManagePatient GET - Missing patientId");
                return BadRequest();
            }

            try
            {
                var patient = await _patients.GetByIdAsync(patientId.Value);
                if (patient == null)
                {
                    _logger.LogWarning("[AppointmentsController] ManagePatient GET - No patient found for ID {PatientId}", patientId);
                    return NotFound();
                }

                var appointments = (await _appointments.GetAllWithRelationsAsync() ?? Enumerable.Empty<Appointment>())
                    .Where(a => a.PatientId == patientId.Value)
                    .OrderBy(a => a.AvailableDay?.Date)
                    .ToList();

                ViewData["PatientMode"] = true;
                ViewData["SelectedPatient"] = patient;

                return View("Manage", appointments);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AppointmentsController] ManagePatient GET failed for patientId {PatientId}", patientId);
                return View("Error", new { message = "Failed to load patient appointments." });
            }
        }
    }
}