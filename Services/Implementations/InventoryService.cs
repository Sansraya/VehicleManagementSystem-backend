using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext dbContext;

        public InventoryService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<InventoryPartDto>> GetPartsInventoryAsync()
        {
            return await dbContext.Parts
                .Include(p => p.Vendor)
                .Select(p => new InventoryPartDto
                {
                    PartsId = p.PartsId,
                    Name = p.Name,
                    PartCode = p.PartCode,
                    Category = p.Category,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    StockStatus = p.StockQuantity == 0 ? "Out of Stock"
                                : p.StockQuantity < 10 ? "Low Stock"
                                : "In Stock",
                    Vendor = p.Vendor.Name,
                    UpdatedAt = p.UpdatedAt
                })
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryVehicleDto>> GetVehicleInventoryAsync()
        {
            return await dbContext.Vehicles
                .Select(v => new InventoryVehicleDto
                {
                    VehicleId = v.VehicleId,
                    Brand = v.Brand,
                    Model = v.Model,
                    VehicleNumber = v.VehicleNumber,
                    Colour = v.Colour,
                    ManufacturingYear = v.ManufacturingYear,
                    FuelType = v.FuelType,
                    Price = v.Price,
                    AvailabilityStatus = v.IsAvailable ? "Available" : "Not Available"
                })
                .ToListAsync();
        }

        public async Task<InventorySummaryDto> GetInventorySummaryAsync()
        {
            var totalParts = await dbContext.Parts.CountAsync();
            var outOfStock = await dbContext.Parts.CountAsync(p => p.StockQuantity == 0);
            var lowStock = await dbContext.Parts.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity < 10);
            var totalVehicles = await dbContext.Vehicles.CountAsync();
            var availableVehicles = await dbContext.Vehicles.CountAsync(v => v.IsAvailable);

            return new InventorySummaryDto
            {
                Parts = new PartsSummaryDto
                {
                    Total = totalParts,
                    OutOfStock = outOfStock,
                    LowStock = lowStock,
                    InStock = totalParts - outOfStock - lowStock
                },
                Vehicles = new VehiclesSummaryDto
                {
                    Total = totalVehicles,
                    Available = availableVehicles,
                    NotAvailable = totalVehicles - availableVehicles
                }
            };
        }
    }
}