using Microsoft.EntityFrameworkCore;
using System;

namespace HomeCareAppointment.Models
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            AvailableDayDbContext context = serviceScope.ServiceProvider.GetRequiredService<AvailableDayDbContext>();

            // Reset database for testing
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed Personnel
            if (!context.Personnels.Any())
            {
                var personnelList = new List<Personnel>
                {
                    new Personnel { Name = "Dr. Alice Smith" },
                    new Personnel { Name = "Nurse Bob Johnson" },
                    new Personnel { Name = "Therapist Carol Lee" }
                };

                context.Personnels.AddRange(personnelList);
                context.SaveChanges();
            }

            // Seed AvailableDays
            if (!context.AvailableDays.Any())
            {
                var availableDays = new List<AvailableDay>
                {
                    new AvailableDay
                    {
                        PersonnelId = 1,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        IsBooked = false
                    },
                    new AvailableDay
                    {
                        PersonnelId = 1,
                        Date = DateTime.Today.AddDays(2),
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0),
                        IsBooked = false
                    },
                    new AvailableDay
                    {
                        PersonnelId = 2,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0),
                        IsBooked = true
                    },
                    new AvailableDay
                    {
                        PersonnelId = 3,
                        Date = DateTime.Today.AddDays(3),
                        StartTime = new TimeSpan(8, 30, 0),
                        EndTime = new TimeSpan(11, 30, 0),
                        IsBooked = false
                    }
                };

                context.AvailableDays.AddRange(availableDays);
                context.SaveChanges();
            }
        }
    }
}
