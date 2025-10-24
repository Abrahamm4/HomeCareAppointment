using System.ComponentModel.DataAnnotations;
using HomeCareAppointment.Attributes;
namespace HomeCareAppointment.Models
{
    [AvailableDayRange] //Validation for entire class, for StartTime<EndTime
    public class AvailableDay
    {
        public int Id { get; set; }

        // Foreign key to Personnel
        public int PersonnelId { get; set; }

        // Navigation to Personnel
        public virtual Personnel? Personnel { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm\\:ss}")]
        public TimeSpan StartTime { get; set; }
        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString ="{0:hh\\:mm\\:ss}")]
        public TimeSpan EndTime { get; set; }

        // One-to-one: an AvailableDay has zero or one Appointment (slot)

        public bool IsBooked { get; set; }
        // Computed property: true if any appointment exists
       // public bool IsBooked => Appointments.Any();
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
