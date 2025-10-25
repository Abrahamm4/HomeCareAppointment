using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public interface IAppointmentRepository
    {
        // Basis CRUD/spørringer
        Task<IEnumerable<Appointment>?> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>?> GetByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>?> GetByPersonnelAsync(int personnelId);
        Task<bool> CreateAsync(Appointment appt);
        Task<bool> UpdateAsync(Appointment appt);
        Task<bool> DeleteAsync(int id);

        // "Med relasjoner" - brukes av AppointmentsController (Details/Index)
        Task<IEnumerable<Appointment>?> GetAllWithRelationsAsync();
        Task<Appointment?> GetByIdWithRelationsAsync(int id);
    }
}
