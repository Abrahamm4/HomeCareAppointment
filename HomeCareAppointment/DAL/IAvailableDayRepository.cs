using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public interface IAvailableDayRepository
    {
        Task<IEnumerable<AvailableDay>> GetAllAsync();
        Task<AvailableDay?> GetByIdAsync(int id);
        Task<IEnumerable<AvailableDay>> GetByPersonnelAsync(int personnelId);
        Task<IEnumerable<AvailableDay>> GetByDateAsync(DateTime date);
        Task CreateAsync(AvailableDay day);
        Task UpdateAsync(AvailableDay day);
        Task<bool> DeleteAsync(int id);
    }
}