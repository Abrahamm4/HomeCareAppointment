using Microsoft.AspNetCore.Mvc;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;
using System.Threading.Tasks;

namespace HomeCareAppointment.Controllers
{
    [Route("[controller]")] // => /Patient/...
    public class PatientController : Controller
    {
        private readonly IPatientRepository _repo;
        public PatientController(IPatientRepository repo) => _repo = repo;

        // Svarer på både /Patient og /Patient/Table
        [HttpGet("")]          // GET /Patient
        [HttpGet("Table")]     // GET /Patient/Table
        public async Task<IActionResult> Index()
        {
            var patients = await _repo.GetAllAsync();
            return View(patients); // bruker Views/Patient/Index.cshtml
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpGet("Create")]
        public IActionResult Create() => View(new Patient());

        [HttpPost("Create"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost("Edit"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost("Delete"), ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}