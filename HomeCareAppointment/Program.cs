using HomeCareAppointment.DAL;
using HomeCareAppointment.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace HomeCareAppointment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog: logging til ny fil med timestamp for hver kjøring
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File($"Logs/app_{timestamp}.log")
                .Filter.ByExcluding(e =>
                    e.Properties.TryGetValue("SourceContext", out var value) &&
                    e.Level == LogEventLevel.Information &&
                    e.MessageTemplate.Text.Contains("Executed DbCommand"))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger: Log.Logger, dispose: true);

            // Console-logging i utvikling
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddConsole();
            }

            builder.Services.AddControllersWithViews();

            // DbContext + SQLite
            builder.Services.AddDbContext<AvailableDayDbContext>(options =>
                options.UseLazyLoadingProxies()
                       .UseSqlite(builder.Configuration["ConnectionStrings:AvailableDayDbContextConnection"]));

            // Repositories
            builder.Services.AddScoped<IAvailableDayRepository, AvailableDayRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPersonnelRepository, PersonnelRepository>();

            var app = builder.Build();

            // Database connection test
            try
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AvailableDayDbContext>();
                db.Database.CanConnect();
                Console.WriteLine("Database connection established!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
            }

            // HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Seed kun hvis databasen er tom
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AvailableDayDbContext>();
                if (!db.Patients.Any())
                {
                    DBInit.Seed(app);
                }
            }

            // Print port info til konsollen
            var urls = app.Urls.Any() ? string.Join(", ", app.Urls) : "Default Kestrel port 5000";
            Console.WriteLine($"Application listening on: {urls}");

            app.Run();
        }
    }
}
