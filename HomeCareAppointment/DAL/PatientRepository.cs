using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AvailableDayDbContext _db;
        public PatientRepository(AvailableDayDbContext db) => _db = db;

        public async Task<IEnumerable<Patient>> GetAllAsync() =>
            await _db.Patients.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

        public async Task<Patient?> GetByIdAsync(int id) =>
            await _db.Patients.FindAsync(id);

        public async Task CreateAsync(Patient patient)
        {
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Patients.FindAsync(id);
            if (entity == null) return false;
            _db.Patients.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}