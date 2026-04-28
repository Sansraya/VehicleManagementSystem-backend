using Microsoft.AspNetCore.Identity;

namespace VehicleManagementSystem.Entities;

public class Vehicle
{
    public Guid VehicleId { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public required string VehicleNumber { get; set; }
    public required string Colour { get; set; }
    public required DateOnly ManufacturingYear { get; set; }
    public required string FuelType { get; set; }
    public required double Price { get; set; }
    public required bool IsAvailable { get; set; }
}