using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public interface IAvailableDayRepository
    {
        // Grunnleggende CRUD
        Task<IEnumerable<AvailableDay>> GetAllAsync();
        Task<AvailableDay?> GetByIdAsync(int id);
        Task<IEnumerable<AvailableDay>> GetByPersonnelAsync(int personnelId);
        Task<IEnumerable<AvailableDay>> GetByDateAsync(DateTime date);
        Task CreateAsync(AvailableDay day);
        Task UpdateAsync(AvailableDay day);
        Task<bool> DeleteAsync(int id);

        // Med relasjoner (brukes i controllerne)
        Task<IEnumerable<AvailableDay>> GetAllWithRelationsAsync();
        Task<AvailableDay?> GetByIdWithRelationsAsync(int id);
    }
}