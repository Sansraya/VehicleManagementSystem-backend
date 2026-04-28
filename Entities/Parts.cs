using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities
{
    public class Parts
    {
        public Guid PartsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("VendorId")]
        public Guid VendorId { get; set; }
        public Vendor? Vendor { get; set; } 

    }
}
