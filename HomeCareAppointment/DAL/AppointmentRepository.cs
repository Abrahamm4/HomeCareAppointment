using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AvailableDayDbContext _db;
        public AppointmentRepository(AvailableDayDbContext db) => _db = db;

        // --- Basis ---

        public async Task<IEnumerable<Appointment>> GetAllAsync() =>
            await _db.Appointments
                     .AsNoTracking()
                     .OrderBy(a => a.Date)
                     .ToListAsync();

        public async Task<Appointment?> GetByIdAsync(int id) =>
            await _db.Appointments
                     .AsNoTracking()
                     .FirstOrDefaultAsync(a => a.AppointmentId == id);

        public async Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId) =>
            await _db.Appointments
                     .AsNoTracking()
                     .Where(a => a.PatientId == patientId)
                     .OrderBy(a => a.Date)
                     .ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByPersonnelAsync(int personnelId) =>
            await _db.Appointments
                     .AsNoTracking()
                     .Where(a => a.PersonnelId == personnelId)
                     .OrderBy(a => a.Date)
                     .ToListAsync();

        public async Task CreateAsync(Appointment appt)
        {
            _db.Appointments.Add(appt);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appt)
        {
            _db.Appointments.Update(appt);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Appointments.FindAsync(id);
            if (entity == null) return false;
            _db.Appointments.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // --- Med relasjoner ---

        public async Task<IEnumerable<Appointment>> GetAllWithRelationsAsync() =>
            await _db.Appointments
                     .Include(a => a.AvailableDay)
                     .Include(a => a.Patient)
                     .Include(a => a.Personnel)
                     .AsNoTracking()
                     .OrderBy(a => a.Date)
                     .ToListAsync();

        public async Task<Appointment?> GetByIdWithRelationsAsync(int id) =>
            await _db.Appointments
                     .Include(a => a.AvailableDay)
                     .Include(a => a.Patient)
                     .Include(a => a.Personnel)
                     .FirstOrDefaultAsync(a => a.AppointmentId == id);
    }
}