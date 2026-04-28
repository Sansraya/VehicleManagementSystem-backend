namespace VehicleManagementSystem.Dtos
{
    public class PartDto
    {
        public Guid PartsId { get; set; }
        public string? Name { get; set; }
        public string? PartCode { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public VendorSummaryDto? Vendor { get; set; }
    }

    public class VendorSummaryDto
    {
        public Guid VendorId { get; set; }
        public string? Name { get; set; }
    }
}