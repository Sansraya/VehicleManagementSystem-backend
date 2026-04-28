using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpGet("parts")]
        public async Task<IActionResult> GetPartsInventory()
        {
            var result = await inventoryService.GetPartsInventoryAsync();
            return Ok(result);
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicleInventory()
        {
            var result = await inventoryService.GetVehicleInventoryAsync();
            return Ok(result);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetInventorySummary()
        {
            var result = await inventoryService.GetInventorySummaryAsync();
            return Ok(result);
        }
    }
}