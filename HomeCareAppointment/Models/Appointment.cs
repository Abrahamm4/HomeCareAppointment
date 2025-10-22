using System.ComponentModel.DataAnnotations;

namespace HomeCareAppointment.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }


        // Extra info

        [Required]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        // Foreign keys
        public int? PatientId { get; set; }
        public int PersonnelId { get; set; }
        public int AvailableDayId { get; set; }

        // Navigation properties (nullable to avoid reference-type mismatches)
        public virtual Patient? Patient { get; set; }
        public virtual Personnel? Personnel { get; set; }
        public virtual AvailableDay? AvailableDay { get; set; }
    }
}
