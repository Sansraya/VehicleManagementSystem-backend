using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Entities
{
    public class PartRequest
    {
        [Key]
        public Guid PartRequestId { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        public User? Customer { get; set; }

        public string PartName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    }
}