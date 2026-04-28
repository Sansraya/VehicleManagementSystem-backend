namespace VehicleManagementSystem.Dtos.Response;

public class CreateVehicleDto
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string VehicleNumber { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public DateOnly ManufacturingYear { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public double Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class UpdateVehicleDto
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Colour { get; set; }
    public string? FuelType { get; set; }
    public double? Price { get; set; }
    public bool? IsAvailable { get; set; }
}

public class VehicleResponseDto
{
    public Guid VehicleId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string VehicleNumber { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public DateOnly ManufacturingYear { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public double Price { get; set; }
    public bool IsAvailable { get; set; }
}