using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;
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
    }
}