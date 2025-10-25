using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeCareAppointment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeCareAppointment.DAL
{
    public class AvailableDayRepository : IAvailableDayRepository
    {
        private readonly AvailableDayDbContext _db;
        private readonly ILogger<AvailableDayRepository> _logger;

        public AvailableDayRepository(AvailableDayDbContext db, ILogger<AvailableDayRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // --- Grunnleggende CRUD ---
        public async Task<IEnumerable<AvailableDay>?> GetAllAsync()
        {
            try
            {
                var list = await _db.AvailableDays.AsNoTracking().ToListAsync();
                return list.OrderBy(d => d.Date).ThenBy(d => d.StartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] GetAllAsync failed");
                return null;
            }
        }

        public async Task<AvailableDay?> GetByIdAsync(int id)
        {
            try
            {
                return await _db.AvailableDays.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] GetByIdAsync failed for Id {AvailableDayId:0000}", id);
                return null;
            }
        }

        public async Task<IEnumerable<AvailableDay>?> GetByPersonnelAsync(int personnelId)
        {
            try
            {
                var list = await _db.AvailableDays
                                    .AsNoTracking()
                                    .Where(d => d.PersonnelId == personnelId)
                                    .ToListAsync();
                return list.OrderBy(d => d.Date).ThenBy(d => d.StartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] GetByPersonnelAsync failed for PersonnelId {PersonnelId:0000}", personnelId);
                return null;
            }
        }

        public async Task<IEnumerable<AvailableDay>?> GetByDateAsync(DateTime date)
        {
            try
            {
                var start = date.Date;
                var end = start.AddDays(1);

                var list = await _db.AvailableDays
                                    .AsNoTracking()
                                    .Where(d => d.Date >= start && d.Date < end)
                                    .ToListAsync();

                return list.OrderBy(d => d.StartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] GetByDateAsync failed for Date {Date}", date);
                return null;
            }
        }

        public async Task<bool> CreateAsync(AvailableDay day)
        {
            try
            {
                _db.AvailableDays.Add(day);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] CreateAsync failed {@AvailableDay}", day);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(AvailableDay day)
        {
            try
            {
                _db.AvailableDays.Update(day);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] UpdateAsync failed {@AvailableDay}", day);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _db.AvailableDays.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogError("[AvailableDayRepository] DeleteAsync failed, not found Id {AvailableDayId:0000}", id);
                    return false;
                }

                _db.AvailableDays.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] DeleteAsync failed for Id {AvailableDayId:0000}", id);
                return false;
            }
        }

        // --- Med relasjoner ---
        public async Task<IEnumerable<AvailableDay>?> GetAllWithRelationsAsync()
        {
            try
            {
                var list = await _db.AvailableDays
                                    .Include(d => d.Personnel)
                                    .Include(d => d.Appointment)
                                    .AsNoTracking()
                                    .ToListAsync();
                return list.OrderBy(d => d.Date).ThenBy(d => d.StartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] GetAllWithRelationsAsync failed");
                return null;
            }
        }

        public async Task<AvailableDay?> GetByIdWithRelationsAsync(int id)
        {
            try
            {
                return await _db.AvailableDays
                                .Include(d => d.Personnel)
                                .Include(d => d.Appointment)
                                .FirstOrDefaultAsync(d => d.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AvailableDayRepository] GetByIdWithRelationsAsync failed for Id {AvailableDayId:0000}", id);
                return null;
            }
        }
    }
}
