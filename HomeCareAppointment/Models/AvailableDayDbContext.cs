using Microsoft.EntityFrameworkCore;

namespace HomeCareAppointment.Models
{
    public class AvailableDayDbContext : DbContext
    {
        public DbSet<AvailableDay> AvailableDays { get; set; }
        public DbSet<Personnel> Personnels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public AvailableDayDbContext(DbContextOptions<AvailableDayDbContext> options)
            : base(options)
        {
        }
    }
}
