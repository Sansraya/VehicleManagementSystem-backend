using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities;

public class Sale
{
    public Guid SaleId { get; set; }

    [ForeignKey("CustomerId")]
    public string CustomerId { get; set; } = string.Empty;
    public required User Customer { get; set; }

    [ForeignKey("StaffId")]
    public string StaffId { get; set; } = string.Empty;
    public required User Staff { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
    public string? Remarks { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public Invoice? Invoice { get; set; }
}