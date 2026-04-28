using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Response;

namespace VehicleManagementSystem.Services;

public interface IVehicleService
{
    Task<List<VehicleResponseDto>> GetAllVehicleAsync(
        bool? available, string? brand, string? fuelType, string? plateNumber);

    Task<VehicleResponseDto?> GetVehicleByIdAsync(Guid id);

    Task<(VehicleResponseDto vehicle, bool alreadyExists)> CreateVehicleAsync(CreateVehicleDto dto);

    Task<(bool found, bool updated)> UpdateVehicleAsync(Guid id, UpdateVehicleDto dto);

    Task<(bool found, bool hasSales)> DeleteVehicleAsync(Guid id);
    
}