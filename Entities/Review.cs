using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public Customer? Customer { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}