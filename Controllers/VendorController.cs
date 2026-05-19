using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Services.Implementations;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IVendorService vendorService;

        public VendorController(ApplicationDbContext dbContext, IVendorService vendorService)
        {
            this.dbContext = dbContext;
            this.vendorService = vendorService;
        }

        [HttpPost("AddPartByVendor")]
        public async Task<IActionResult> AddPartByVendor(AddPartByVendorDTO vendorDTO)
        {
            var response = await vendorService.AddPart(vendorDTO);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }
        [HttpPost("UpdatePartByVendor/{partId}")]
        public async Task<IActionResult> UpdatePartByVendor(Guid partId, AddPartByVendorDTO vendorDTO)
        {
            var response = await vendorService.UpdatePart(partId, vendorDTO);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }
        [HttpDelete("DeletePartByVendor/{partId}")]
        public async Task<IActionResult> DeletePartByVendor(Guid partId)
        {
            var response = await vendorService.DeletePart(partId);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }

    }
}