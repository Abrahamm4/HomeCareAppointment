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
    public class AvailableDaysController : Controller
    {
        private readonly AvailableDayDbContext _context;

        public AvailableDaysController(AvailableDayDbContext context)
        {
            _context = context;
        }

        // GET: AvailableDays
        public async Task<IActionResult> Index()
        {
            var availableDayDbContext = _context.AvailableDays
                .Include(a => a.Personnel)
                .Include(a => a.Appointment);
            return View(await availableDayDbContext.ToListAsync());
        }

        // GET: AvailableDays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableDay = await _context.AvailableDays
                .Include(a => a.Personnel)
                .Include(a => a.Appointment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (availableDay == null)
            {
                return NotFound();
            }

            return View(availableDay);
        }

        // GET: AvailableDays/Create
        public IActionResult Create()
        {
            ViewData["PersonnelId"] = new SelectList(_context.Personnels, "Id", "Name");
            return View();
        }

        // POST: AvailableDays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonnelId,Date,StartTime,EndTime")] AvailableDay availableDay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(availableDay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonnelId"] = new SelectList(_context.Personnels, "Id", "Name", availableDay.PersonnelId);
            return View(availableDay);
        }

        // GET: AvailableDays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableDay = await _context.AvailableDays.FindAsync(id);
            if (availableDay == null)
            {
                return NotFound();
            }
            ViewData["PersonnelId"] = new SelectList(_context.Personnels, "Id", "Name", availableDay.PersonnelId);
            return View(availableDay);
        }

        // POST: AvailableDays/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonnelId,Date,StartTime,EndTime")] AvailableDay availableDay)
        {
            if (id != availableDay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availableDay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailableDayExists(availableDay.Id))
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
            ViewData["PersonnelId"] = new SelectList(_context.Personnels, "Id", "Name", availableDay.PersonnelId);
            return View(availableDay);
        }

        // GET: AvailableDays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableDay = await _context.AvailableDays
                .Include(a => a.Personnel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (availableDay == null)
            {
                return NotFound();
            }

            return View(availableDay);
        }

        // POST: AvailableDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availableDay = await _context.AvailableDays.FindAsync(id);
            if (availableDay != null)
            {
                _context.AvailableDays.Remove(availableDay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvailableDayExists(int id)
        {
            return _context.AvailableDays.Any(e => e.Id == id);
        }
    }
}
