namespace VehicleManagementSystem.Dtos;

public class SaleItemRequestDto
{
    public Guid? VehicleId { get; set; }   // Provide either VehicleId OR PartsId
    public Guid? PartsId { get; set; }
    public int Quantity { get; set; }
}