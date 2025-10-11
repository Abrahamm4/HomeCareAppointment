namespace HomeCareAppointment.Models
{
    public class Personnel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // Navigation property (one-to-many)
        public virtual ICollection<AvailableDay>? AvailableDays { get; set; }
    }
}
