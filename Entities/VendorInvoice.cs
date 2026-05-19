using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities
{
 
    public class VendorInvoice
    {
        [Key]
        public Guid VendorInvoiceId { get; set; } = Guid.NewGuid();

        public string InvoiceNumber { get; set; } = string.Empty;

        
        [ForeignKey("VendorAssignment")]
        public Guid VendorAssignmentId { get; set; }
        public required VendorAssignment VendorAssignment { get; set; }

        
        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }
        public required Vendor Vendor { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;
        public User? Customer { get; set; }

        
        public string Status { get; set; } = "Issued";

        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public ICollection<VendorInvoiceItem> Items { get; set; } = new List<VendorInvoiceItem>();
    }

    public class VendorInvoiceItem
    {
        [Key]
        public Guid VendorInvoiceItemId { get; set; } = Guid.NewGuid();

        [ForeignKey("VendorInvoice")]
        public Guid VendorInvoiceId { get; set; }
        public VendorInvoice? VendorInvoice { get; set; }

        public string Description { get; set; } = string.Empty;

        public string ItemType { get; set; } = "Service";

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }

        public decimal SubTotal => Quantity * UnitPrice;
    }
}