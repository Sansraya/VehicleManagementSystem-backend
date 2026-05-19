using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/staff/customer-reports")]
    [ApiController]
    public class CustomerReportController : ControllerBase
    {
        private readonly ICustomerReportService reportService;

        public CustomerReportController(ICustomerReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] int regularThreshold = 3,
            [FromQuery] decimal highSpenderThreshold = 50000)
        {
            var result = await reportService.GetSummaryAsync(regularThreshold, highSpenderThreshold);
            return Ok(result);
        }

        [HttpGet("regulars")]
        public async Task<IActionResult> GetRegulars([FromQuery] int minVisits = 3)
        {
            var result = await reportService.GetRegularCustomersAsync(minVisits);
            return Ok(result);
        }

        [HttpGet("high-spenders")]
        public async Task<IActionResult> GetHighSpenders([FromQuery] decimal minSpent = 50000)
        {
            var result = await reportService.GetHighSpendersAsync(minSpent);
            return Ok(result);
        }

        [HttpGet("pending-credits")]
        public async Task<IActionResult> GetPendingCredits()
        {
            var result = await reportService.GetPendingCreditsAsync();
            return Ok(result);
        }
    }
}