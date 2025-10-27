using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.Controllers
{
    public class PersonnelController : Controller
    {
        private readonly IPersonnelRepository _personnels;
        private readonly ILogger<PersonnelController> _logger;

        public PersonnelController(IPersonnelRepository personnels, ILogger<PersonnelController> logger)
        {
            _personnels = personnels;
            _logger = logger;
        }

        // GET: /Personnel
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _personnels.GetAllAsync() ?? Enumerable.Empty<Personnel>();
                return View(list);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] Index failed");
                return View("Error", new { message = "Failed to load personnel list." });
            }
        }

        // GET: /Personnel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("[PersonnelController] Details called with null Id");
                return NotFound();
            }

            try
            {
                var personnel = await _personnels.GetByIdAsync(id.Value);
                if (personnel == null)
                {
                    _logger.LogWarning("[PersonnelController] Details: No personnel found for Id {PersonnelId}", id);
                    return NotFound();
                }
                return View(personnel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] Details failed for Id {PersonnelId}", id);
                return BadRequest();
            }
        }

        // GET: /Personnel/Create
        public IActionResult Create() => View(new Personnel());

        // POST: /Personnel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Personnel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[PersonnelController] Create called with invalid model {@Personnel}", model);
                return View(model);
            }

            try
            {
                var ok = await _personnels.CreateAsync(model);
                if (!ok)
                {
                    _logger.LogError("[PersonnelController] Create failed for {@Personnel}", model);
                    ModelState.AddModelError(string.Empty, "Could not create personnel.");
                    return View(model);
                }

                _logger.LogInformation("[PersonnelController] Created new personnel {@Personnel}", model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] Create POST failed");
                return BadRequest();
            }
        }

        // GET: /Personnel/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("[PersonnelController] Edit(GET) called with null Id");
                return NotFound();
            }

            try
            {
                var personnel = await _personnels.GetByIdAsync(id.Value);
                if (personnel == null)
                {
                    _logger.LogWarning("[PersonnelController] Edit(GET): No personnel found for Id {PersonnelId}", id);
                    return NotFound();
                }
                return View(personnel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] Edit(GET) failed for Id {PersonnelId}", id);
                return BadRequest();
            }
        }

        // POST: /Personnel/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Personnel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning("[PersonnelController] Edit POST Id mismatch: route={RouteId}, model={ModelId}", id, model.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[PersonnelController] Edit POST invalid model {@Personnel}", model);
                return View(model);
            }

            try
            {
                var ok = await _personnels.UpdateAsync(model);
                if (!ok)
                {
                    _logger.LogError("[PersonnelController] Edit POST failed for {@Personnel}", model);
                    ModelState.AddModelError(string.Empty, "Could not update personnel.");
                    return View(model);
                }

                _logger.LogInformation("[PersonnelController] Updated personnel {@Personnel}", model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] Edit POST failed for Id {PersonnelId}", id);
                return BadRequest();
            }
        }

        // GET: /Personnel/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("[PersonnelController] Delete(GET) called with null Id");
                return NotFound();
            }

            try
            {
                var personnel = await _personnels.GetByIdAsync(id.Value);
                if (personnel == null)
                {
                    _logger.LogWarning("[PersonnelController] Delete(GET): No personnel found for Id {PersonnelId}", id);
                    return NotFound();
                }
                return View(personnel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] Delete(GET) failed for Id {PersonnelId}", id);
                return BadRequest();
            }
        }

        // POST: /Personnel/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ok = await _personnels.DeleteAsync(id);
                if (!ok)
                {
                    _logger.LogWarning("[PersonnelController] DeleteConfirmed failed: No personnel found for Id {PersonnelId}", id);
                    var entity = await _personnels.GetByIdAsync(id);
                    ModelState.AddModelError(string.Empty, "Could not delete personnel.");
                    return View("Delete", entity);
                }

                _logger.LogInformation("[PersonnelController] Deleted personnel with Id {PersonnelId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelController] DeleteConfirmed failed for Id {PersonnelId}", id);
                return BadRequest();
            }
        }
    }
}
