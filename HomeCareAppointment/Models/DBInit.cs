using Microsoft.EntityFrameworkCore;
using System;

namespace HomeCareAppointment.Models
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AvailableDayDbContext>();

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

            // Seed Patients
            //if (!context.Patients.Any())
            //{
            //    var patients = new List<Patient>
            //    {
            //        new Patient { FullName = "John Doe", Email = "john@example.com", PhoneNumber = "12345678" },
            //        new Patient { FullName = "Mary Johnson", Email = "mary@example.com", PhoneNumber = "87654321" }
            //    };

            //    context.Patients.AddRange(patients);
            //    context.SaveChanges();
            //}

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
                        EndTime = new TimeSpan(12, 0, 0)
                    },
                    new AvailableDay
                    {
                        PersonnelId = 1,
                        Date = DateTime.Today.AddDays(2),
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0)
                    },
                    new AvailableDay
                    {
                        PersonnelId = 2,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0)
                    },
                    new AvailableDay
                    {
                        PersonnelId = 3,
                        Date = DateTime.Today.AddDays(3),
                        StartTime = new TimeSpan(8, 30, 0),
                        EndTime = new TimeSpan(11, 30, 0)
                    }
                };

                context.AvailableDays.AddRange(availableDays);
                context.SaveChanges();
            }

            // Seed ONE Appointment (linking Patient + Personnel + AvailableDay)
            if (!context.Appointments.Any())
            {
                var appointment = new Appointment
                {
                    //PatientId = context.Patients.First().Id,       // John Doe
                    PersonnelId = context.Personnels.First().Id,   // Dr. Alice Smith
                    AvailableDayId = context.AvailableDays.First().Id, // First slot
                    Notes = "Medication reminder"
                };

                context.Appointments.Add(appointment);
                context.SaveChanges();
            }

            // Seed ONE Patient 
            if (!context.Patients.Any())
            {
                var patient = new Patient
                {
                    PatientId = 1,
                    Name= "Test",
                    Phone = "92017932",
                    Email="test@gmail.com"
                };

            }
        }
    }
}
