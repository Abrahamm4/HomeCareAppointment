using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.Controllers
{
    public class AvailableDaysController : Controller
    {
        private readonly IAvailableDayRepository _days;
        private readonly IPersonnelRepository _personnel;

        public AvailableDaysController(
            IAvailableDayRepository days,
            IPersonnelRepository personnel)
        {
            _days = days;
            _personnel = personnel;
        }

        // GET: AvailableDays
        public async Task<IActionResult> Index()
        {
            var list = await _days.GetAllWithRelationsAsync();
            return View(list);
        }

        // GET: AvailableDays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            var day = await _days.GetByIdWithRelationsAsync(id.Value);
            if (day is null) return NotFound();
            return View(day);
        }

        // GET: AvailableDays/Create
        public async Task<IActionResult> Create()
        {
            var personnels = await _personnel.GetAllAsync();
            ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name");
            return View();
        }

        // POST: AvailableDays/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonnelId,Date,StartTime,EndTime")] AvailableDay model)
        {
            if (!ModelState.IsValid)
            {
                var personnels = await _personnel.GetAllAsync();
                ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name", model.PersonnelId);
                return View(model);
            }

            await _days.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: AvailableDays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();
            var model = await _days.GetByIdAsync(id.Value);
            if (model is null) return NotFound();

            var personnels = await _personnel.GetAllAsync();
            ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name", model.PersonnelId);
            return View(model);
        }

        // POST: AvailableDays/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonnelId,Date,StartTime,EndTime")] AvailableDay model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var personnels = await _personnel.GetAllAsync();
                ViewData["PersonnelId"] = new SelectList(personnels, "Id", "Name", model.PersonnelId);
                return View(model);
            }

            await _days.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: AvailableDays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var day = await _days.GetByIdWithRelationsAsync(id.Value);
            if (day is null) return NotFound();
            return View(day);
        }

        // POST: AvailableDays/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _days.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AvailableDayExists(int id)
        {
            var d = await _days.GetByIdAsync(id);
            return d != null;
        }
    }
}