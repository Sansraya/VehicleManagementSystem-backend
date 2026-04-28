using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public InventoryController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("parts")]
        public async Task<IActionResult> GetPartsInventory()
        {
            var parts = await dbContext.Parts
                .Include(p => p.Vendor)
                .Select(p => new
                {
                    p.PartsId,
                    p.Name,
                    p.PartCode,
                    p.Category,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    StockStatus = p.StockQuantity == 0 ? "Out of Stock"
                                : p.StockQuantity < 10 ? "Low Stock"
                                : "In Stock",
                    Vendor = p.Vendor.Name,
                    p.UpdatedAt
                })
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            return Ok(parts);
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicleInventory()
        {
            var vehicles = await dbContext.Vehicles
                .Select(v => new
                {
                    v.VehicleId,
                    v.Brand,
                    v.Model,
                    v.VehicleNumber,
                    v.Colour,
                    v.ManufacturingYear,
                    v.FuelType,
                    v.Price,
                    AvailabilityStatus = v.IsAvailable ? "Available" : "Not Available"
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetInventorySummary()
        {
            var totalParts = await dbContext.Parts.CountAsync();
            var outOfStock = await dbContext.Parts.CountAsync(p => p.StockQuantity == 0);
            var lowStock = await dbContext.Parts.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity < 10);
            var totalVehicles = await dbContext.Vehicles.CountAsync();
            var availableVehicles = await dbContext.Vehicles.CountAsync(v => v.IsAvailable);

            return Ok(new
            {
                Parts = new
                {
                    Total = totalParts,
                    OutOfStock = outOfStock,
                    LowStock = lowStock,
                    InStock = totalParts - outOfStock - lowStock
                },
                Vehicles = new
                {
                    Total = totalVehicles,
                    Available = availableVehicles,
                    NotAvailable = totalVehicles - availableVehicles
                }
            });
        }
    }
}