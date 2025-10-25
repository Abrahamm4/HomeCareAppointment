using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>?> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Patient patient);
        Task<bool> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int id);
    }
}
