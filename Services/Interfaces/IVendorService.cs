using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Models;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IVendorService
    {
        Task<(bool Success, string Message, Guid? VendorId)> CreateVendorAsync(VendorModel model);
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto?> GetVendorByIdAsync(Guid id);
        Task<(bool Success, string Message)> UpdateVendorAsync(Guid id, VendorModel model);
        Task<(bool Success, string Message)> DeleteVendorAsync(Guid id);
    }
}