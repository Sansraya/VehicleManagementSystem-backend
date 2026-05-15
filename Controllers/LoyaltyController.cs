using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/loyalty")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        [HttpPost("calculate-discount")]
        public IActionResult CalculateDiscount(LoyaltyDiscountDto dto)
        {
            if (dto.TotalAmount <= 0)
            {
                return BadRequest(new
                {
                    message = "Total amount must be greater than 0."
                });
            }

            decimal discountAmount = 0;
            bool isEligible = false;

            if (dto.TotalAmount > 5000)
            {
                discountAmount = dto.TotalAmount * 0.10m;
                isEligible = true;
            }

            decimal finalAmount = dto.TotalAmount - discountAmount;

            var response = new LoyaltyDiscountResponseDto
            {
                TotalAmount = dto.TotalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                IsEligible = isEligible,
                Message = isEligible
                    ? "Customer is eligible for 10% loyalty discount."
                    : "Customer is not eligible for loyalty discount."
            };

            return Ok(response);
        }
    }
}