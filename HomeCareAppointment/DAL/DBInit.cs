using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeCareAppointment.Models;

namespace HomeCareAppointment.DAL
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AvailableDayDbContext>();

            // Reset database for testing (destructive). Kept intentionally for local testing.
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
            if (!context.Patients.Any())
            {
                var patients = new List<Patient>
                {
                    new Patient { Name = "John Doe", Email = "john@example.com", Phone = "12345678" },
                    new Patient { Name = "Mary Johnson", Email = "mary@example.com", Phone = "87654321" },
                    new Patient { Name = "Test", Email = "test@gmail.com", Phone = "92017932" }
                };

                context.Patients.AddRange(patients);
                context.SaveChanges();
            }

            // Seed AvailableDays
            if (!context.AvailableDays.Any())
            {
                // Ensure there are personnels to reference
                var personnels = context.Personnels.ToList();
                if (!personnels.Any()) return; // nothing to seed against

                var availableDays = new List<AvailableDay>
                {
                    new AvailableDay
                    {
                        PersonnelId = personnels.ElementAt(0).Id,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0)
                    },
                    new AvailableDay
                    {
                        PersonnelId = personnels.ElementAt(0).Id,
                        Date = DateTime.Today.AddDays(2),
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0)
                    },
                    new AvailableDay
                    {
                        PersonnelId = personnels.ElementAt(1).Id,
                        Date = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0)
                    },
                    new AvailableDay
                    {
                        PersonnelId = personnels.ElementAt(2).Id,
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
                var firstAvailableDay = context.AvailableDays.FirstOrDefault();
                var firstPatient = context.Patients.FirstOrDefault();
                var firstPersonnel = context.Personnels.FirstOrDefault();

                if (firstAvailableDay != null && firstPatient != null && firstPersonnel != null)
                {
                    var appointment = new Appointment
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
