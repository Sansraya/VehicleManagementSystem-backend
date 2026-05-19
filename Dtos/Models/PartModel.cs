using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Dtos.Models
{
    public class PartModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string PartCode { get; set; }

        [Required]
        public required string Category { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int StockQuantity { get; set; }

        [Required]
        public Guid VendorId { get; set; }
    }
}