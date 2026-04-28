using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/part-requests")]
    [ApiController]
    public class PartRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create part request
        [HttpPost]
        public async Task<IActionResult> CreateRequest(PartRequestDto dto)
        {
            var request = new PartRequest
            {
                CustomerId = dto.CustomerId,
                PartName = dto.PartName,
                Description = dto.Description
            };

            _context.PartRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        // Get requests by customer
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetRequests(string customerId)
        {
            var requests = await _context.PartRequests
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return Ok(requests);
        }
    }
}