using System.ComponentModel.DataAnnotations;
using HomeCareAppointment.Attributes;
namespace HomeCareAppointment.Models
{
    [AvailableDayRange] //Validation for entire class, for StartTime<EndTime
    public class AvailableDay
    {
        public int Id { get; set; }

        // Foreign key
        public int PersonnelId { get; set; }

        // Navigation property
        public virtual Personnel? Personnel { get; set; }


        [Required(ErrorMessage = "Date required")]
        [FutureDate] //Custom validation for future dates
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }


        public bool IsBooked { get; set; }
        // Computed property: true if any appointment exists
       // public bool IsBooked => Appointments.Any();
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
