using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/admin/financial-reports")]
    [ApiController]
    public class FinancialReportController : ControllerBase
    {
        private readonly IFinancialReportService reportService;

        public FinancialReportController(IFinancialReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily([FromQuery] DateTime? date)
        {
            var target = date ?? DateTime.UtcNow;
            var result = await reportService.GetDailyReportAsync(target);
            return Ok(result);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var y = year ?? now.Year;
            var m = month ?? now.Month;
            if (m < 1 || m > 12) return BadRequest(new { message = "Month must be between 1 and 12." });
            var result = await reportService.GetMonthlyReportAsync(y, m);
            return Ok(result);
        }

        [HttpGet("yearly")]
        public async Task<IActionResult> GetYearly([FromQuery] int? year)
        {
            var y = year ?? DateTime.UtcNow.Year;
            var result = await reportService.GetYearlyReportAsync(y);
            return Ok(result);
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            if (end < start) return BadRequest(new { message = "End date must be after start date." });
            var result = await reportService.GetRangeReportAsync(start, end);
            return Ok(result);
        }
    }
}