using Microsoft.AspNetCore.Identity;

namespace VehicleManagementSystem.Entities;

public class Vehicle : IdentityUser
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public string VehicleNumber { get; set; }
    public string Colour { get; set; }
    public DateOnly ManufacturingYear { get; set; }
    public string FuelType { get; set; }
    public double Price { get; set; }
    public bool IsAvailable { get; set; }
}