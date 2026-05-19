using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IVendorService
    {
        public Task<VendorResponse> AddPart(AddPartByVendorDTO vendorDTO);
        public Task<VendorResponse> UpdatePart(Guid partId, AddPartByVendorDTO vendorDTO);
        public Task<VendorResponse> DeletePart(Guid partId);
        Task<(bool Success, string Message, Guid? VendorId)> CreateVendorAsync(VendorModel model);
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto?> GetVendorByIdAsync(Guid id);
        Task<(bool Success, string Message)> UpdateVendorAsync(Guid id, VendorModel model);
        Task<(bool Success, string Message)> DeleteVendorAsync(Guid id);

    }
}
