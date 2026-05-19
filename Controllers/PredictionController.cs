using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IPredictionService predictionService;
 
        public PredictionController(IPredictionService predictionService)
        {
            this.predictionService = predictionService;
        }
        
        [HttpPost("predict-failures")]
        public async Task<IActionResult> PredictPartFailures([FromBody] VehiclePredictionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
 
            var (success, message, result) =
                await predictionService.PredictPartFailuresAsync(dto);
 
            if (!success)
                return BadRequest(new { message });
 
            return Ok(result);
        }
}