using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCareAppointment.DAL
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AvailableDayDbContext>();

            // VIKTIG: Bruk migrasjoner, ikke EnsureCreated/EnsureDeleted
            context.Database.Migrate();

            // ----- Personnel -----
            if (!context.Personnels.Any())
            {
                var personnelList = new List<Models.Personnel>
                {
                    new Models.Personnel { Name = "Dr. Alice Smith" },
                    new Models.Personnel { Name = "Nurse Bob Johnson" },
                    new Models.Personnel { Name = "Therapist Carol Lee" }
                };
                context.Personnels.AddRange(personnelList);
                context.SaveChanges();
            }

            // ----- Patients -----
            if (!context.Patients.Any())
            {
                var patients = new List<Models.Patient>
                {
                    new Models.Patient { Name = "John Doe",  Email = "john@example.com",  Phone = "12345678" },
                    new Models.Patient { Name = "Mary Johnson", Email = "mary@example.com", Phone = "87654321" },
                    new Models.Patient { Name = "Test", Email = "test@gmail.com", Phone = "92017932" }
                };
                context.Patients.AddRange(patients);
                context.SaveChanges();
            }

            // Hent persisterte nøkler
            var personnelsList = context.Personnels.ToList();
            if (!personnelsList.Any()) return;

            // ----- AvailableDays -----
            if (!context.AvailableDays.Any())
            {
                var availableDays = new List<Models.AvailableDay>
                {
                    new Models.AvailableDay
                    {
                        PersonnelId = personnelsList.ElementAt(0).Id,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0)
                    },
                    new Models.AvailableDay
                    {
                        PersonnelId = personnelsList.ElementAt(0).Id,
                        Date = DateTime.Today.AddDays(2),
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0)
                    },
                    new Models.AvailableDay
                    {
                        PersonnelId = personnelsList.ElementAt(1).Id,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0)
                    },
                    new Models.AvailableDay
                    {
                        PersonnelId = personnelsList.ElementAt(2).Id,
                        Date = DateTime.Today.AddDays(3),
                        StartTime = new TimeSpan(8, 30, 0),
                        EndTime = new TimeSpan(11, 30, 0)
                    }
                };
                context.AvailableDays.AddRange(availableDays);
                context.SaveChanges();
            }

            // ----- Én Appointment (valgfritt) -----
            if (!context.Appointments.Any())
            {
                var firstAvailableDay = context.AvailableDays.FirstOrDefault();
                var firstPatient = context.Patients.FirstOrDefault();
                var firstPersonnel = context.Personnels.FirstOrDefault();
                if (firstAvailableDay != null && firstPatient != null && firstPersonnel != null)
                {
                    var appointment = new Models.Appointment
                    {
                        PatientId = firstPatient.PatientId,
                        PersonnelId = firstPersonnel.Id,
                        AvailableDayId = firstAvailableDay.Id,
                        Date = firstAvailableDay.Date,
                        Notes = "Medication reminder"
                    };
                    context.Appointments.Add(appointment);
                    context.SaveChanges();
                }
            }
        }
    }
}