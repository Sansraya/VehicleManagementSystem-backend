namespace VehicleManagementSystem.Dtos
{
    public class InventoryPartDto
    {
        public Guid PartsId { get; set; }
        public string? Name { get; set; }
        public string? PartCode { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? StockStatus { get; set; }
        public string? Vendor { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class InventoryVehicleDto
    {
        public Guid VehicleId { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Colour { get; set; }
        public DateOnly ManufacturingYear { get; set; }
        public string? FuelType { get; set; }
        public double Price { get; set; }
        public string? AvailabilityStatus { get; set; }
    }

    public class InventorySummaryDto
    {
        public PartsSummaryDto? Parts { get; set; }
        public VehiclesSummaryDto? Vehicles { get; set; }
    }

    public class PartsSummaryDto
    {
        public int Total { get; set; }
        public int OutOfStock { get; set; }
        public int LowStock { get; set; }
        public int InStock { get; set; }
    }

    public class VehiclesSummaryDto
    {
        public int Total { get; set; }
        public int Available { get; set; }
        public int NotAvailable { get; set; }
    }
}