using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest(new
                {
                    message = "Rating must be between 1 and 5."
                });
            }

            var customerExists = await _context.Users.AnyAsync(c => c.Id == dto.CustomerId);

            if (!customerExists)
            {
                return BadRequest(new
                {
                    message = "Invalid customerId. Customer does not exist."
                });
            }

            var review = new Review
            {
                CustomerId = dto.CustomerId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReviews(string customerId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return Ok(reviews);
        }
    }
}