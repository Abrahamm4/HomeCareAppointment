using HomeCareAppointment.DAL;     // <- Repo + DbContext ligger i DAL
using HomeCareAppointment.Models;  // <- Hvis brukt andre steder; kan fjernes om ubrukt
using Microsoft.EntityFrameworkCore;
using Serilog;                     // <- Serilog for logging
using Serilog.Events;             // <- For filtrering av loggnivåer

namespace HomeCareAppointment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // === Serilog-konfigurasjon ===
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information() // levels: Trace < Information < Warning < Error < Fatal
                .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

            // Filtrer ut EF Core "Executed DbCommand"-logging på Information-nivå
            loggerConfiguration.Filter.ByExcluding(e =>
                e.Properties.TryGetValue("SourceContext", out var value) &&
                e.Level == LogEventLevel.Information &&
                e.MessageTemplate.Text.Contains("Executed DbCommand"));

            var logger = loggerConfiguration.CreateLogger();
            builder.Logging.ClearProviders(); // Valgfritt: fjerner standard loggere
            builder.Logging.AddSerilog(logger);

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
