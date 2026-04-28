using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Entities
{
    public class PartRequest
    {
        [Key]
        public int RequestId { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public Customer? Customer { get; set; }

        public string PartName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    }
}