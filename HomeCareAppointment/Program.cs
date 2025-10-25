using HomeCareAppointment.DAL;     // <- Repo + DbContext ligger i DAL
using HomeCareAppointment.Models;  // <- Hvis brukt andre steder; kan fjernes om ubrukt
using Microsoft.EntityFrameworkCore;

namespace HomeCareAppointment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DbContext + Lazy Loading Proxies + SQLite
            builder.Services.AddDbContext<AvailableDayDbContext>(options =>
            {
                options
                    .UseLazyLoadingProxies() // Viktig: flytt proxies hit, ellers blir de ikke aktivert
                    .UseSqlite(builder.Configuration["ConnectionStrings:AvailableDayDbContextConnection"]);
            });

            // === Repository-registreringer (Scoped, én instans per HTTP-request) ===
            builder.Services.AddScoped<IAvailableDayRepository, AvailableDayRepository>();
            builder.Services.AddScoped<IAppointmentRepository,  AppointmentRepository>();
            builder.Services.AddScoped<IPatientRepository,      PatientRepository>();
            builder.Services.AddScoped<IPersonnelRepository,    PersonnelRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Seeding: KUN i Development (DBInit.Seed sletter/oppretter databasen)
            if (app.Environment.IsDevelopment())
            {
                DBInit.Seed(app);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}