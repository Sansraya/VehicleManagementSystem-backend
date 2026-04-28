using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/part-requests")]
    [ApiController]
    public class PartRequestController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public PartRequestController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(PartRequestDto dto)
        {
            var customerExists = await dbContext.Users.AnyAsync(u => u.Id == dto.CustomerId);

            if (!customerExists)
            {
                return BadRequest(new { message = "Invalid customer ID." });
            }

            var request = new PartRequest
            {
                CustomerId = dto.CustomerId,
                PartName = dto.PartName,
                Description = dto.Description,
                RequestDate = DateTime.UtcNow
            };

            dbContext.PartRequests.Add(request);
            await dbContext.SaveChangesAsync();

            return Ok(request);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetRequests(string customerId)
        {
            var requests = await dbContext.PartRequests
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return Ok(requests);
        }
    }
}