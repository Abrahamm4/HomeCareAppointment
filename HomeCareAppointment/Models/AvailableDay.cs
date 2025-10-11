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

        //  public ICollection<Appointment>? Appointments { get; set; }
    }
}
