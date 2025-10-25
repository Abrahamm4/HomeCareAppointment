// DAL/PersonnelRepository.cs
using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public class PersonnelRepository : IPersonnelRepository
    {
        private readonly AvailableDayDbContext _db;
        public PersonnelRepository(AvailableDayDbContext db) => _db = db;

        public async Task<IEnumerable<Personnel>> GetAllAsync() =>
            await _db.Personnels.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

        public async Task<Personnel?> GetByIdAsync(int id) =>
            await _db.Personnels.FindAsync(id);
    }
}
