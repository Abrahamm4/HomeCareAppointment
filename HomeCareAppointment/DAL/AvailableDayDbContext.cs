using Microsoft.EntityFrameworkCore;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public class AvailableDayDbContext : DbContext
    {
        public DbSet<AvailableDay> AvailableDays { get; set; }
        public DbSet<Personnel> Personnels { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Patient> Patients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        public AvailableDayDbContext(DbContextOptions<AvailableDayDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // konfigerer one-to-one: AvailableDay <-> Appointment
            modelBuilder.Entity<AvailableDay>()
                .HasOne(d => d.Appointment)
                .WithOne(a => a.AvailableDay)
                .HasForeignKey<Appointment>(a => a.AvailableDayId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
