namespace VehicleManagementSystem.Dtos
{
    public class ReviewResponseDto
    {
        public Guid ReviewId { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; }
    }
}