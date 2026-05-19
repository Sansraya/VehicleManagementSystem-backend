using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class VendorsService : IVendorService
    {
        private readonly ApplicationDbContext dbContext;

        public VendorsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, Guid? VendorId)> CreateVendorAsync(VendorModel model)
        {
            var vendor = new Vendor
            {
                VendorId = Guid.NewGuid(),
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Vendors.AddAsync(vendor);
            await dbContext.SaveChangesAsync();
            return (true, "Vendor created", vendor.VendorId);
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            return await dbContext.Vendors
                .Select(v => new VendorDto
                {
                    VendorId = v.VendorId,
                    Name = v.Name,
                    ContactPerson = v.ContactPerson,
                    Email = v.Email,
                    Phone = v.Phone,
                    Address = v.Address,
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<VendorDto?> GetVendorByIdAsync(Guid id)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return null;

            return new VendorDto
            {
                VendorId = vendor.VendorId,
                Name = vendor.Name,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                Phone = vendor.Phone,
                Address = vendor.Address,
                CreatedAt = vendor.CreatedAt
            };
        }

        public async Task<(bool Success, string Message)> UpdateVendorAsync(Guid id, VendorModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return (false, "Vendor not found");

            vendor.Name = model.Name;
            vendor.ContactPerson = model.ContactPerson;
            vendor.Email = model.Email;
            vendor.Phone = model.Phone;
            vendor.Address = model.Address;

            await dbContext.SaveChangesAsync();
            return (true, "Vendor updated");
        }

        public async Task<(bool Success, string Message)> DeleteVendorAsync(Guid id)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return (false, "Vendor not found");

            dbContext.Vendors.Remove(vendor);
            await dbContext.SaveChangesAsync();
            return (true, "Vendor deleted");
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