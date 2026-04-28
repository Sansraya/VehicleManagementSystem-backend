using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interface
{
    public interface IVendorService
    {
        public Task<VendorResponse> AddPart(AddPartByVendorDTO vendorDTO);
        public Task<VendorResponse> UpdatePart(Guid partId, AddPartByVendorDTO vendorDTO);
        public Task<VendorResponse> DeletePart(Guid partId);

    }
}
