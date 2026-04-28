namespace VehicleManagementSystem.Dtos.Response;

public class SaleResponseDto
{
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string InvoiceStatus { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }

    public string StaffName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    public List<SaleItemResponseDto> Items { get; set; } = new();

    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
}