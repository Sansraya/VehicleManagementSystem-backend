using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;

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
                .ToListAsync();

            var partRequests = await dbContext.PartRequests
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.RequestDate)
                .ToListAsync();

            var reviews = await dbContext.Reviews
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReviewDate)
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