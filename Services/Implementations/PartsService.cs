using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class PartsService : IPartsService
    {
        private readonly ApplicationDbContext dbContext;

        public PartsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, Guid? PartsId)> CreatePartAsync(PartModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
            if (vendor == null) return (false, "Vendor not found", null);

            var part = new Parts
            {
                PartsId = Guid.NewGuid(),
                Name = model.Name,
                PartCode = model.PartCode,
                Category = model.Category,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                VendorId = model.VendorId,
                Vendor = vendor,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await dbContext.Parts.AddAsync(part);
            await dbContext.SaveChangesAsync();
            return (true, "Part created", part.PartsId);
        }

        public async Task<IEnumerable<PartDto>> GetAllPartsAsync()
        {
            return await dbContext.Parts
                .Include(p => p.Vendor)
                .Select(p => new PartDto
                {
                    PartsId = p.PartsId,
                    Name = p.Name,
                    PartCode = p.PartCode,
                    Category = p.Category,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Vendor = new VendorSummaryDto
                    {
                        VendorId = p.Vendor.VendorId,
                        Name = p.Vendor.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<PartDto?> GetPartByIdAsync(Guid id)
        {
            var part = await dbContext.Parts
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(p => p.PartsId == id);

            if (part == null) return null;

            return new PartDto
            {
                PartsId = part.PartsId,
                Name = part.Name,
                PartCode = part.PartCode,
                Category = part.Category,
                Description = part.Description,
                Price = part.Price,
                StockQuantity = part.StockQuantity,
                CreatedAt = part.CreatedAt,
                UpdatedAt = part.UpdatedAt,
                Vendor = new VendorSummaryDto
                {
                    VendorId = part.Vendor.VendorId,
                    Name = part.Vendor.Name
                }
            };
        }

        public async Task<(bool Success, string Message)> UpdatePartAsync(Guid id, PartModel model)
        {
            var part = await dbContext.Parts.FindAsync(id);
            if (part == null) return (false, "Part not found");

            if (model.VendorId != part.VendorId)
            {
                var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
                if (vendor == null) return (false, "Vendor not found");
            }

            part.Name = model.Name;
            part.PartCode = model.PartCode;
            part.Category = model.Category;
            part.Description = model.Description;
            part.Price = model.Price;
            part.StockQuantity = model.StockQuantity;
            part.VendorId = model.VendorId;
            part.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return (true, "Part updated");
        }

        public async Task<(bool Success, string Message)> DeletePartAsync(Guid id)
        {
            var part = await dbContext.Parts.FindAsync(id);
            if (part == null) return (false, "Part not found");

            dbContext.Parts.Remove(part);
            await dbContext.SaveChangesAsync();
            return (true, "Part deleted");
        }
    }
}