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

        // Navigation property: one patient can have many appointments
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
