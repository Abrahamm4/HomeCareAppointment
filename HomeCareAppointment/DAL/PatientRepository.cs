using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;
using Microsoft.Extensions.Logging;

namespace HomeCareAppointment.DAL
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AvailableDayDbContext _db;
        private readonly ILogger<PatientRepository> _logger;

        public PatientRepository(AvailableDayDbContext db, ILogger<PatientRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Patient>?> GetAllAsync()
        {
            try
            {
                return await _db.Patients.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientRepository] GetAllAsync failed");
                return null;
            }
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            try
            {
                return await _db.Patients.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientRepository] GetByIdAsync failed for Id {PatientId:0000}", id);
                return null;
            }
        }

        public async Task<bool> CreateAsync(Patient patient)
        {
            try
            {
                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientRepository] CreateAsync failed {@Patient}", patient);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            try
            {
                _db.Patients.Update(patient);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientRepository] UpdateAsync failed {@Patient}", patient);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _db.Patients.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError("[PatientRepository] DeleteAsync failed, not found Id {PatientId:0000}", id);
                    return false;
                }

                _db.Patients.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PatientRepository] DeleteAsync failed for Id {PatientId:0000}", id);
                return false;
            }
        }
    }
}
