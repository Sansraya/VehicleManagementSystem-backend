using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/customer-history")]
    [ApiController]
    public class CustomerHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerHistory(string customerId)
        {
            var customerExists = await _context.Users.AnyAsync(u => u.Id == customerId);

            if (!customerExists)
            {
                return NotFound(new
                {
                    message = "Customer not found."
                });
            }

            var appointments = await _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            var partRequests = await _context.PartRequests
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.RequestDate)
                .ToListAsync();

            var reviews = await _context.Reviews
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