namespace VehicleManagementSystem.Models
{
    public class ReviewDto
    {
        public string CustomerId { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}