namespace VehicleManagementSystem.Dtos
{
    public class ReviewDto
    {
        public string CustomerId { get; set; } = string.Empty;

        public Guid? AppointmentId { get; set; }

        public Guid? PartId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}