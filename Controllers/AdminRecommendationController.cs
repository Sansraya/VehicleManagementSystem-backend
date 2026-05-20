using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers;

[Route("api/admin/recommendations")]
[ApiController]
public class AdminRecommendationController : ControllerBase
{
    private readonly IAdminRecommendationService recommendationService;

    public AdminRecommendationController(IAdminRecommendationService recommendationService)
    {
        this.recommendationService = recommendationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecommendations()
    {
        var (success, message, result) = await recommendationService.GetRecommendationsAsync();
        if (!success)
            return BadRequest(new { message });
        return Ok(result);
    }
}