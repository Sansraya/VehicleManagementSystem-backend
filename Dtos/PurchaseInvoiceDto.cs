namespace VehicleManagementSystem.Dtos
{
    public class PurchaseInvoiceDto
    {
        public Guid PurchaseInvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public VendorSummaryDto? Vendor { get; set; }
        public List<PurchaseInvoiceItemDto> Items { get; set; } = new();
    }

    public class PurchaseInvoiceItemDto
    {
        public Guid PartsId { get; set; }
        public string? PartName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}