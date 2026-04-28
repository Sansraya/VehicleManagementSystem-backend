namespace VehicleManagementSystem.Dtos
{
    public class AddPartByVendorDTO
    {
        public Guid Vendorid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
