using System.ComponentModel.DataAnnotations;

namespace HomeCareAppointment.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        // Relationships
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
    }
}
