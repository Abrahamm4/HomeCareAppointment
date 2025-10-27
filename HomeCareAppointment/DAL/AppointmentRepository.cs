using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AvailableDayDbContext _db;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(AvailableDayDbContext db, ILogger<AppointmentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        //Basis CRUD

        public async Task<IEnumerable<Appointment>?> GetAllAsync()
        {
            try
            {
                return await _db.Appointments
                                .AsNoTracking()
                                .OrderBy(a => a.Date)
                                .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] GetAllAsync failed: {Message}", e.Message);
                return null;
            }
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            try
            {
                return await _db.Appointments
                                .AsNoTracking()
                                .FirstOrDefaultAsync(a => a.AppointmentId == id);
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] GetByIdAsync failed for Id {Id}: {Message}", id, e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Appointment>?> GetByPatientAsync(int patientId)
        {
            try
            {
                return await _db.Appointments
                                .AsNoTracking()
                                .Where(a => a.PatientId == patientId)
                                .OrderBy(a => a.Date)
                                .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] GetByPatientAsync failed for PatientId {PatientId}: {Message}", patientId, e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Appointment>?> GetByPersonnelAsync(int personnelId)
        {
            try
            {
                return await _db.Appointments
                                .AsNoTracking()
                                .Where(a => a.PersonnelId == personnelId)
                                .OrderBy(a => a.Date)
                                .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] GetByPersonnelAsync failed for PersonnelId {PersonnelId}: {Message}", personnelId, e.Message);
                return null;
            }
        }

        public async Task<bool> CreateAsync(Appointment appt)
        {
            try
            {
                _db.Appointments.Add(appt);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] CreateAsync failed for Appointment {@Appointment}: {Message}", appt, e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Appointment appt)
        {
            try
            {
                _db.Appointments.Update(appt);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] UpdateAsync failed for Appointment {@Appointment}: {Message}", appt, e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _db.Appointments.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("[AppointmentRepository] DeleteAsync: Appointment not found for Id {Id}", id);
                    return false;
                }

                _db.Appointments.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] DeleteAsync failed for Id {Id}: {Message}", id, e.Message);
                return false;
            }
        }

        //Med relasjoner

        public async Task<IEnumerable<Appointment>?> GetAllWithRelationsAsync()
        {
            try
            {
                return await _db.Appointments
                                .Include(a => a.AvailableDay)
                                .Include(a => a.Patient)
                                .Include(a => a.Personnel)
                                .AsNoTracking()
                                .OrderBy(a => a.Date)
                                .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] GetAllWithRelationsAsync failed: {Message}", e.Message);
                return null;
            }
        }

        public async Task<Appointment?> GetByIdWithRelationsAsync(int id)
        {
            try
            {
                return await _db.Appointments
                                .Include(a => a.AvailableDay)
                                .Include(a => a.Patient)
                                .Include(a => a.Personnel)
                                .FirstOrDefaultAsync(a => a.AppointmentId == id);
            }
            catch (Exception e)
            {
                _logger.LogError("[AppointmentRepository] GetByIdWithRelationsAsync failed for Id {Id}: {Message}", id, e.Message);
                return null;
            }
        }
    }
}
