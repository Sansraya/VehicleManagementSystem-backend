using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/customer-history")]
    [ApiController]
    public class CustomerHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerHistoryController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerHistory(string customerId)
        {
            var customerExists = await dbContext.Users.AnyAsync(u => u.Id == customerId);

            if (!customerExists)
            {
                return NotFound(new { message = "Customer not found." });
            }

            var appointments = await dbContext.Appointments
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentResponseDto
                {
                    AppointmentId = a.AppointmentId,
                    CustomerId = a.CustomerId,
                    AppointmentDate = a.AppointmentDate,
                    ServiceType = a.ServiceType,
                    Description = a.Description
                })
                .ToListAsync();

            var partRequests = await dbContext.PartRequests
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.RequestDate)
                .Select(p => new PartRequestResponseDto
                {
                    PartRequestId = p.PartRequestId,
                    CustomerId = p.CustomerId,
                    PartName = p.PartName,
                    Description = p.Description,
                    RequestDate = p.RequestDate
                })
                .ToListAsync();

            var reviews = await dbContext.Reviews
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    CustomerId = r.CustomerId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();

            return Ok(new
            {
                customerId,
                appointments,
                partRequests,
                reviews
            });
        }
    }
}