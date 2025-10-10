using System.ComponentModel.DataAnnotations;

namespace HomeCareAppointment.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public virtual List<Appointment>? Appointments { get; set; }
    }
}
