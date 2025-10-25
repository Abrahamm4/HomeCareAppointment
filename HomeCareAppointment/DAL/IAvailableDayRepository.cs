using HomeCareAppointment.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCareAppointment.DAL
{
    public interface IAvailableDayRepository
    {
        //Basis CRUD
        Task<IEnumerable<AvailableDay>?> GetAllAsync();
        Task<AvailableDay?> GetByIdAsync(int id);
        Task<IEnumerable<AvailableDay>?> GetByPersonnelAsync(int personnelId);
        Task<IEnumerable<AvailableDay>?> GetByDateAsync(DateTime date);

        Task<bool> CreateAsync(AvailableDay day);
        Task<bool> UpdateAsync(AvailableDay day);
        Task<bool> DeleteAsync(int id);

        //Med relasjoner
        Task<IEnumerable<AvailableDay>?> GetAllWithRelationsAsync();
        Task<AvailableDay?> GetByIdWithRelationsAsync(int id);
    }
}
