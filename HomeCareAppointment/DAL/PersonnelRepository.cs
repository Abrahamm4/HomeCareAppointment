using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCareAppointment.DAL
{
    public class PersonnelRepository : IPersonnelRepository
    {
        private readonly AvailableDayDbContext _db;
        private readonly ILogger<PersonnelRepository> _logger;

        public PersonnelRepository(AvailableDayDbContext db, ILogger<PersonnelRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        //Read 
        public async Task<IEnumerable<Personnel>?> GetAllAsync()
        {
            try
            {
                var list = await _db.Personnels.AsNoTracking().ToListAsync();
                return list.OrderBy(p => p.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelRepository] GetAllAsync failed");
                return null;
            }
        }

        public async Task<Personnel?> GetByIdAsync(int id)
        {
            try
            {
                return await _db.Personnels.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelRepository] GetByIdAsync failed for Id {Id}", id);
                return null;
            }
        }

        //Create
        public async Task<bool> CreateAsync(Personnel personnel)
        {
            try
            {
                _db.Personnels.Add(personnel);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelRepository] CreateAsync failed {@Personnel}", personnel);
                return false;
            }
        }

        //Update
        public async Task<bool> UpdateAsync(Personnel personnel)
        {
            try
            {
                _db.Personnels.Update(personnel);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelRepository] UpdateAsync failed {@Personnel}", personnel);
                return false;
            }
        }

        //Delete 
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _db.Personnels.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("[PersonnelRepository] DeleteAsync failed, Id not found {Id}", id);
                    return false;
                }

                _db.Personnels.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PersonnelRepository] DeleteAsync failed for Id {Id}", id);
                return false;
            }
        }
    }
}
