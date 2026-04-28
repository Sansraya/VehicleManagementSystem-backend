using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Response;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services;

namespace VehicleManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        this.vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? available,
        [FromQuery] string? brand,
        [FromQuery] string? fuelType,
        [FromQuery] string? plateNumber)
    {
        var vehicles = await vehicleService.GetAllVehicleAsync(available, brand, fuelType, plateNumber);
        return Ok(vehicles);
    }

    [HttpGet("GetAllVehicle")]
    public async Task<IActionResult> GetAllVehicle(bool? available, string? brand, string? fuelType, string? plateNumber)
    {
        List<VehicleResponseDto> vehicles = await vehicleService.GetAllVehicleAsync(available, brand, fuelType, plateNumber);

        return Ok(vehicles);
    }
    

    [HttpGet("GetVehicleById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var vehicle = await vehicleService.GetVehicleByIdAsync(id);
        if (vehicle is null)
            return NotFound(new { message = "Vehicle not found." });

        return Ok(vehicle);
    }

    [HttpPost("CreateVehicle")]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto dto)
    {
        var (vehicle, alreadyExists) = await vehicleService.CreateVehicleAsync(dto);
        if (alreadyExists)
            return Conflict(new { message = $"Vehicle with plate '{dto.VehicleNumber}' already exists." });

        return CreatedAtAction(nameof(GetById), new { id = vehicle.VehicleId }, vehicle);
    }

    [HttpPut("UpdateVehicle/{id:guid}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        var (found, _) = await vehicleService.UpdateVehicleAsync(id, dto);
        if (!found)
            return NotFound(new { message = "Vehicle not found." });

        return Ok(new { message = "Vehicle updated successfully." });
    }

    [HttpDelete("DeleteVehicle/{id:guid}")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var (found, hasSales) = await vehicleService.DeleteVehicleAsync(id);
        if (!found)
            return NotFound(new { message = "Vehicle not found." });
        if (hasSales)
            return Conflict(new { message = "Cannot delete a vehicle with sales records. Mark it unavailable instead." });

        return Ok(new { message = "Vehicle deleted successfully." });
    }
    
}