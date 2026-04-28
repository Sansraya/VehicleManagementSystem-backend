using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ReviewController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest(new { message = "Rating must be between 1 and 5." });
            }

            var customerExists = await dbContext.Users.AnyAsync(u => u.Id == dto.CustomerId);

            if (!customerExists)
            {
                return BadRequest(new { message = "Invalid customer ID." });
            }

            var review = new Review
            {
                CustomerId = dto.CustomerId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.UtcNow
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();

            var response = new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                CustomerId = review.CustomerId,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate
            };

            return Ok(response);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReviews(string customerId)
        {
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

            return Ok(reviews);
        }
    }
}