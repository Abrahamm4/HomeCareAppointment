// DAL/IPersonnelRepository.cs
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public interface IPersonnelRepository
    {
        Task<IEnumerable<Personnel>> GetAllAsync();
        Task<Personnel?> GetByIdAsync(int id);
    }
}