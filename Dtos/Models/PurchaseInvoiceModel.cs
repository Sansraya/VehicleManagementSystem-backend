using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Dtos.Models
{
    public class PurchaseInvoiceModel
    {
        [Required]
        public Guid VendorId { get; set; }

        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public required List<PurchaseInvoiceItemModel> Items { get; set; }
    }

    public class PurchaseInvoiceItemModel
    {
        [Required]
        public Guid PartsId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}