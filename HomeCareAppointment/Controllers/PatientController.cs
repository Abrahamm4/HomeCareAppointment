using Microsoft.AspNetCore.Mvc;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HomeCareAppointment.Controllers
{
    [Route("[controller]")] // -> /Patient/...
    public class PatientController : Controller
    {
        private readonly IPatientRepository _repo;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientRepository repo, ILogger<PatientController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Table")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var patients = await _repo.GetAllAsync();
                return View(patients);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] Index failed");
                return View("Error", new { message = "Failed to load patients." });
            }
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var patient = await _repo.GetByIdAsync(id);
                if (patient == null) return NotFound();
                return View(patient);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] Details failed for Id {PatientId}", id);
                return BadRequest();
            }
        }

        [HttpGet("Create")]
        public IActionResult Create() => View(new Patient());

        [HttpPost("Create"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var success = await _repo.CreateAsync(model);
                if (!success)
                {
                    _logger.LogError("[PatientController] Create failed for {@Patient}", model);
                    return BadRequest();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] Create POST failed");
                return BadRequest();
            }
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var patient = await _repo.GetByIdAsync(id);
                if (patient == null) return NotFound();
                return View(patient);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] Edit(GET) failed for Id {PatientId}", id);
                return BadRequest();
            }
        }

        [HttpPost("Edit"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var success = await _repo.UpdateAsync(model);
                if (!success)
                {
                    _logger.LogError("[PatientController] Edit POST failed for {@Patient}", model);
                    return BadRequest();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] Edit POST failed");
                return BadRequest();
            }
        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var patient = await _repo.GetByIdAsync(id);
                if (patient == null) return NotFound();
                return View(patient);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] Delete(GET) failed for Id {PatientId}", id);
                return BadRequest();
            }
        }

        [HttpPost("Delete"), ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _repo.DeleteAsync(id);
                if (!success)
                    _logger.LogWarning("[PatientController] DeleteConfirmed: Patient not found for Id {PatientId}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientController] DeleteConfirmed failed for Id {PatientId}", id);
                return BadRequest();
            }
        }
    }
}
