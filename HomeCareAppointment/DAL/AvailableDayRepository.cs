using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeCareAppointment.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCareAppointment.DAL
{
    public class AvailableDayRepository : IAvailableDayRepository
    {
        private readonly AvailableDayDbContext _db;
        public AvailableDayRepository(AvailableDayDbContext db) => _db = db;

        // --- Grunnleggende CRUD ---

        public async Task<IEnumerable<AvailableDay>> GetAllAsync()
        {
            var list = await _db.AvailableDays
                                .AsNoTracking()
                                .ToListAsync(); // hent først
            return list.OrderBy(d => d.Date)
                       .ThenBy(d => d.StartTime); // sorter i minnet (TimeSpan-safe)
        }

        public async Task<AvailableDay?> GetByIdAsync(int id) =>
            await _db.AvailableDays.FindAsync(id);

        public async Task<IEnumerable<AvailableDay>> GetByPersonnelAsync(int personnelId)
        {
            var list = await _db.AvailableDays
                                .AsNoTracking()
                                .Where(d => d.PersonnelId == personnelId)
                                .ToListAsync();
            return list.OrderBy(d => d.Date)
                       .ThenBy(d => d.StartTime); // sorter i minnet
        }

        public async Task<IEnumerable<AvailableDay>> GetByDateAsync(DateTime date)
        {
            // Robust interval-filtrering: [00:00, 24:00)
            var start = date.Date;
            var end = start.AddDays(1);

            var list = await _db.AvailableDays
                                .AsNoTracking()
                                .Where(d => d.Date >= start && d.Date < end)
                                .ToListAsync();

            return list.OrderBy(d => d.StartTime); // sorter i minnet
        }

        public async Task CreateAsync(AvailableDay day)
        {
            _db.AvailableDays.Add(day);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(AvailableDay day)
        {
            _db.AvailableDays.Update(day);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.AvailableDays.FindAsync(id);
            if (entity == null) return false;
            _db.AvailableDays.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // --- Med relasjoner ---

        public async Task<IEnumerable<AvailableDay>> GetAllWithRelationsAsync()
        {
            var list = await _db.AvailableDays
                                .Include(d => d.Personnel)
                                .Include(d => d.Appointment)
                                .AsNoTracking()
                                .ToListAsync(); // hent først
            return list.OrderBy(d => d.Date)
                       .ThenBy(d => d.StartTime); // sorter i minnet
        }

        public async Task<AvailableDay?> GetByIdWithRelationsAsync(int id) =>
            await _db.AvailableDays
                     .Include(d => d.Personnel)
                     .Include(d => d.Appointment)
                     .FirstOrDefaultAsync(d => d.Id == id);
    }
}