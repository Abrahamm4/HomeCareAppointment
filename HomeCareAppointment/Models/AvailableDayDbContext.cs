using Microsoft.EntityFrameworkCore;

namespace HomeCareAppointment.Models
{
    public class AvailableDayDbContext : DbContext
    {
        public DbSet<AvailableDay> AvailableDays { get; set; }

        public AvailableDayDbContext(DbContextOptions<AvailableDayDbContext> options)
            : base(options)
        {
        }
    }
}
