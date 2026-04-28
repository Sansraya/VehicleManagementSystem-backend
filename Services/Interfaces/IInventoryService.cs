using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryPartDto>> GetPartsInventoryAsync();
        Task<IEnumerable<InventoryVehicleDto>> GetVehicleInventoryAsync();
        Task<InventorySummaryDto> GetInventorySummaryAsync();
    }
}