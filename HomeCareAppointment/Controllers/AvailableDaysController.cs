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
            var availableDayDbContext = _context.AvailableDays.Include(a => a.Personnel);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonnelId,Date,StartTime,EndTime,IsBooked")] AvailableDay availableDay)
        {
            if (availableDay.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Date", "Date cannot be a past date");
            }
            if (availableDay.EndTime <= availableDay.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time cannot be before start time");
            }
            var sameDays = await _context.AvailableDays
            .Where(ad => ad.PersonnelId == availableDay.PersonnelId && ad.Date.Date == availableDay.Date.Date)
            .ToListAsync();

            bool overlap = sameDays.Any(ad =>
            {
                if (ad.Id == availableDay.Id) { return false; }
                var parsedStart = TimeSpan.Parse(ad.StartTime.ToString());
                var parsedEnd = TimeSpan.Parse(ad.EndTime.ToString());

                return availableDay.StartTime < parsedEnd && availableDay.EndTime > parsedStart;
            }
            );
            if (overlap)
            {
                ModelState.AddModelError("", "Time slot overlaps with existing time slot for same personnel");
            }
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonnelId,Date,StartTime,EndTime,IsBooked")] AvailableDay availableDay)
        {
            if (id != availableDay.Id)
            {
                return NotFound();
            }
            if (availableDay.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Date", "Date cannot be a past date");
            }
            if (availableDay.EndTime <= availableDay.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time cannot be before start time");
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
