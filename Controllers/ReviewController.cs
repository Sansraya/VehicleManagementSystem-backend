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

            var customerExists = await dbContext.Users
                .AnyAsync(u => u.Id == dto.CustomerId);

            if (!customerExists)
            {
                return BadRequest(new { message = "Invalid customer ID." });
            }

            var vendorExists = await dbContext.Vendors
                .AnyAsync(v => v.VendorId == dto.VendorId);

            if (!vendorExists)
            {
                return BadRequest(new { message = "Invalid vendor ID." });
            }

            if (dto.AppointmentId == null && dto.PartId == null)
            {
                return BadRequest(new
                {
                    message = "Review must be linked to either a vendor-related appointment or a vendor-related part."
                });
            }

            if (dto.AppointmentId != null)
            {
                var appointmentExists = await dbContext.Appointments
                    .AnyAsync(a =>
                        a.AppointmentId == dto.AppointmentId &&
                        a.CustomerId == dto.CustomerId &&
                        a.VendorId == dto.VendorId);

                if (!appointmentExists)
                {
                    return BadRequest(new
                    {
                        message = "Invalid vendor-related appointment for this customer."
                    });
                }
            }

            if (dto.PartId != null)
            {
                var partExists = await dbContext.Parts
                    .AnyAsync(p =>
                        p.PartsId == dto.PartId &&
                        p.VendorId == dto.VendorId);

                if (!partExists)
                {
                    return BadRequest(new
                    {
                        message = "Invalid part ID for this vendor."
                    });
                }
            }

            var review = new Review
            {
                CustomerId = dto.CustomerId,
                VendorId = dto.VendorId,
                AppointmentId = dto.AppointmentId,
                PartId = dto.PartId,
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
                VendorId = review.VendorId,
                AppointmentId = review.AppointmentId,
                PartId = review.PartId,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate
            };

            return Ok(response);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReviewsByCustomer(string customerId)
        {
            var reviews = await dbContext.Reviews
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    CustomerId = r.CustomerId,
                    VendorId = r.VendorId,
                    AppointmentId = r.AppointmentId,
                    PartId = r.PartId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetReviewsByVendor(Guid vendorId)
        {
            var reviews = await dbContext.Reviews
                .Where(r => r.VendorId == vendorId)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    CustomerId = r.CustomerId,
                    VendorId = r.VendorId,
                    AppointmentId = r.AppointmentId,
                    PartId = r.PartId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetReviewsByAppointment(Guid appointmentId)
        {
            var reviews = await dbContext.Reviews
                .Where(r => r.AppointmentId == appointmentId)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    CustomerId = r.CustomerId,
                    VendorId = r.VendorId,
                    AppointmentId = r.AppointmentId,
                    PartId = r.PartId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("part/{partId}")]
        public async Task<IActionResult> GetReviewsByPart(Guid partId)
        {
            var reviews = await dbContext.Reviews
                .Where(r => r.PartId == partId)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    CustomerId = r.CustomerId,
                    VendorId = r.VendorId,
                    AppointmentId = r.AppointmentId,
                    PartId = r.PartId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}