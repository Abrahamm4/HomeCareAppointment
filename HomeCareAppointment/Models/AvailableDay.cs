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

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm\\:ss}")]
        public TimeSpan StartTime { get; set; }

        [Required]        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString ="{0:hh\\:mm\\:ss}")]
        public TimeSpan EndTime { get; set; }

        // One-to-one: an AvailableDay has zero or one Appointment (slot)
        public virtual Appointment? Appointment { get; set; }

        // Optional collection removed: we model a single appointment per slot
    }
}
