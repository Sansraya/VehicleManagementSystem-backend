using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interface;


namespace VehicleManagementSystem.Services.Implementation
{
    public class VendorService:IVendorService
    {
        private readonly ApplicationDbContext dbContext;

        public VendorService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<VendorResponse> AddPart(AddPartByVendorDTO vendorDTO)
        {
            var vendor = await dbContext.Vendors.FindAsync(vendorDTO.Vendorid);
            if (vendor == null)
            {
                return new VendorResponse
                {
                    Success = false,
                    Message = "Vendor not found."
                };
            }

            var part = new Parts
            {
                VendorId = vendor.VendorId,
                Name = vendorDTO.Name,
                PartCode = vendorDTO.PartCode,
                Category = vendorDTO.Category,
                Description = vendorDTO.Description,
                Price = vendorDTO.Price,
                StockQuantity = vendorDTO.StockQuantity,
                CreatedAt = vendorDTO.CreatedAt,
                UpdatedAt = vendorDTO.UpdatedAt
            };

            await dbContext.Parts.AddAsync(part);
            await dbContext.SaveChangesAsync();
            return new VendorResponse
            {
                Success = true,
                Message = "Part added successfully."
            };

        }
        public async Task<VendorResponse> UpdatePart(Guid partId, AddPartByVendorDTO vendorDTO)
        {
            var part = await dbContext.Parts.FindAsync(partId);
            if (part == null)
            {
                return new VendorResponse
                {
                    Success = false,
                    Message = "Part not found."
                };
            }
            part.Name = vendorDTO.Name;
            part.PartCode = vendorDTO.PartCode;
            part.Category = vendorDTO.Category;
            part.Description = vendorDTO.Description;
            part.Price = vendorDTO.Price;
            part.StockQuantity = vendorDTO.StockQuantity;
            part.UpdatedAt = DateTime.UtcNow;
            dbContext.Parts.Update(part);
            await dbContext.SaveChangesAsync();
            return new VendorResponse
            {
                Success = true,
                Message = "Part updated successfully."
            };
        }

        public async Task<VendorResponse> DeletePart(Guid partId)
        {
            var part = await dbContext.Parts.FindAsync(partId);
            if (part == null)
            {
                return new VendorResponse
                {
                    Success = false,
                    Message = "Part not found."
                };
            }
            dbContext.Parts.Remove(part);
            await dbContext.SaveChangesAsync();
            return new VendorResponse
            {
                Success = true,
                Message = "Part deleted successfully."
            };
        }
    }
}
