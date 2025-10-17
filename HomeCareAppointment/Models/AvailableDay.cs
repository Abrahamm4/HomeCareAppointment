namespace HomeCareAppointment.Models
{
    public class AvailableDay
    {
        public int Id { get; set; }

        // Foreign key to Personnel
        public int PersonnelId { get; set; }

        // Navigation to Personnel
        public virtual Personnel? Personnel { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // One-to-one: an AvailableDay has zero or one Appointment (slot)
        public virtual Appointment? Appointment { get; set; }

        // Optional collection removed: we model a single appointment per slot
    }
}
