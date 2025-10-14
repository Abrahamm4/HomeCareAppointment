namespace HomeCareAppointment.Models
{
    public class AvailableDay
    {
        public int Id { get; set; }

        // Foreign key
        public int PersonnelId { get; set; }

        // Navigation property
        public virtual Personnel? Personnel { get; set; }


        
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


        public bool IsBooked { get; set; }
        // Computed property: true if any appointment exists
       // public bool IsBooked => Appointments.Any();
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
