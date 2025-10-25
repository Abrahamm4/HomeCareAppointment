using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task CreateAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int id);
    }
}