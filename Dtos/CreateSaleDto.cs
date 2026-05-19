namespace VehicleManagementSystem.Dtos;

public class CreateSaleDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string StaffId { get; set; } = string.Empty;
    public List<SaleItemRequestDto> Items { get; set; } = new();
    public decimal DiscountAmount { get; set; } = 0;
    public string PaymentMethod { get; set; } = string.Empty;  
    public string? Remarks { get; set; }
}