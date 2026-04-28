using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Entities
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        public User? Customer { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}