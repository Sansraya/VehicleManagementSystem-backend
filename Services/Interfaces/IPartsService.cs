using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Models;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IPartsService
    {
        Task<(bool Success, string Message, Guid? PartsId)> CreatePartAsync(PartModel model);
        Task<IEnumerable<PartDto>> GetAllPartsAsync();
        Task<PartDto?> GetPartByIdAsync(Guid id);
        Task<(bool Success, string Message)> UpdatePartAsync(Guid id, PartModel model);
        Task<(bool Success, string Message)> DeletePartAsync(Guid id);
    }
}