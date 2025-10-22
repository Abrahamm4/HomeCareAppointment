using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public class AvailableDayRepository : IAvailableDayRepository
    {
        private readonly AvailableDayDbContext _db;
        public AvailableDayRepository(AvailableDayDbContext db) => _db = db;

        public async Task<IEnumerable<AvailableDay>> GetAllAsync() =>
            await _db.AvailableDays.AsNoTracking()
                .OrderBy(d => d.Date).ThenBy(d => d.StartTime)
                .ToListAsync();

        public async Task<AvailableDay?> GetByIdAsync(int id) =>
            await _db.AvailableDays.FindAsync(id);

        public async Task<IEnumerable<AvailableDay>> GetByPersonnelAsync(int personnelId) =>
            await _db.AvailableDays.AsNoTracking()
                .Where(d => d.PersonnelId == personnelId)
                .OrderBy(d => d.Date).ThenBy(d => d.StartTime)
                .ToListAsync();

        public async Task<IEnumerable<AvailableDay>> GetByDateAsync(DateTime date) =>
            await _db.AvailableDays.AsNoTracking()
                .Where(d => d.Date.Date == date.Date)
                .OrderBy(d => d.StartTime)
                .ToListAsync();

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
    }
}