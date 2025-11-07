using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;
using Microsoft.Extensions.Logging;

namespace HomeCareAppointment.Controllers
{
    public class AvailableDaysController : Controller
    {
        private readonly IAvailableDayRepository _days;
        private readonly IPersonnelRepository _personnel;
        private readonly ILogger<AvailableDaysController> _logger;

        public AvailableDaysController(
            IAvailableDayRepository days,
            IPersonnelRepository personnel,
            ILogger<AvailableDaysController> logger)
        {
            _days = days;
            _personnel = personnel;
            _logger = logger;
        }

        // GET: AvailableDays
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _days.GetAllWithRelationsAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Index failed");
                return View("Error", "Could not load available days");
            }
        }

        // GET: AvailableDays/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            try
            {
                var day = await _days.GetByIdWithRelationsAsync(id.Value);
                if (day == null)
                {
                    _logger.LogError("[AvailableDaysController] Details: AvailableDay not found for Id {AvailableDayId:0000}", id.Value);
                    return NotFound();
                }
                return View(day);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Details failed for Id {AvailableDayId:0000}", id.Value);
                return BadRequest();
            }
        }

        // GET: AvailableDays/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var personnels = await _personnel.GetAllAsync();
                ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Create GET failed");
                return BadRequest();
            }
        }

        // POST: AvailableDays/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonnelId,Date,StartTime,EndTime")] AvailableDay model)
        {            
            if(model.Date<DateTime.Today){ModelState.AddModelError("Date", "Date cannot be a past date.");}
            if(model.EndTime<=model.StartTime){ModelState.AddModelError("Endtime", "Endtime cannot be before StartTime.");}

            var currentslots=await _days.GetAllAsync() ?? new List<AvailableDay>();
            var sameDaySlots=currentslots.Where(s=>s.PersonnelId==model.PersonnelId&&s.Date.Date==model.Date.Date).ToList();
            bool overlap=sameDaySlots.Any(s=>(model.StartTime<s.EndTime)&&(s.StartTime<model.EndTime));
            if(overlap){ModelState.AddModelError("","Timeslot overlaps with a currently existing timeslot.");}
            if (!ModelState.IsValid)
            {
                var personnels = await _personnel.GetAllAsync();
                ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name", model.PersonnelId);
                return View(model);
            }

            try
            {
                var result = await _days.CreateAsync(model);
                if (!result)
                {
                    _logger.LogError("[AvailableDaysController] Create POST failed {@AvailableDay}", model);
                    return BadRequest();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Create POST exception {@AvailableDay}", model);
                return BadRequest();
            }
        }

        // GET: AvailableDays/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();
            try
            {
                var model = await _days.GetByIdAsync(id.Value);
                if (model == null) return NotFound();

                var personnels = await _personnel.GetAllAsync();
                ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name", model.PersonnelId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Edit GET failed for Id {AvailableDayId:0000}", id.Value);
                return BadRequest();
            }
        }

        // POST: AvailableDays/Edit/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonnelId,Date,StartTime,EndTime")] AvailableDay model)
        {
            if (id != model.Id) return NotFound();
            var currentslots=await _days.GetAllAsync() ?? new List<AvailableDay>();
            var sameDaySlots=currentslots.Where(s=>s.PersonnelId==model.PersonnelId&&s.Date.Date==model.Date.Date).ToList();
            bool overlap=sameDaySlots.Any(s=>(model.StartTime<s.EndTime)&&(s.StartTime<model.EndTime));
            if(overlap){ModelState.AddModelError("","Timeslot overlaps with a currently existing timeslot.");}

            if (!ModelState.IsValid)
            {
                var personnels = await _personnel.GetAllAsync();
                ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name", model.PersonnelId);
                return View(model);
            }

            try
            {
                var result = await _days.UpdateAsync(model);
                if (!result)
                {
                    _logger.LogError("[AvailableDaysController] Edit POST failed {@AvailableDay}", model);
                    return BadRequest();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Edit POST exception {@AvailableDay}", model);
                return BadRequest();
            }
        }

        // GET: AvailableDays/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            try
            {
                var day = await _days.GetByIdWithRelationsAsync(id.Value);
                if (day == null) return NotFound();
                return View(day);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Delete GET failed for Id {AvailableDayId:0000}", id.Value);
                return BadRequest();
            }
        }

        // POST: AvailableDays/Delete/{id}
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _days.DeleteAsync(id);
                if (!result)
                {
                    _logger.LogError("[AvailableDaysController] Delete POST failed for Id {AvailableDayId:0000}", id);
                    return BadRequest();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] Delete POST exception for Id {AvailableDayId:0000}", id);
                return BadRequest();
            }
        }

        private async Task<bool> AvailableDayExists(int id)
        {
            try
            {
                var d = await _days.GetByIdAsync(id);
                return d != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AvailableDaysController] AvailableDayExists check failed for Id {AvailableDayId:0000}", id);
                return false;
            }
        }
    }
}
