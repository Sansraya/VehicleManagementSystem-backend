using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities;

public class Invoice
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;

    [ForeignKey("SaleId")]
    public Guid SaleId { get; set; }
    public required Sale Sale { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Issued";
    public bool EmailSent { get; set; } = false;
    public DateTime? EmailSentAt { get; set; }
}