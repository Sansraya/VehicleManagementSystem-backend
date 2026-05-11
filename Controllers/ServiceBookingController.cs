using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers;

[Route("api/service-bookings")]
[ApiController]
public class ServiceBookingController : ControllerBase
{
    private readonly IServiceBookingService bookingService;
    private readonly UserManager<User> userManager;

    public ServiceBookingController(
        IServiceBookingService bookingService,
        UserManager<User> userManager)
    {
        this.bookingService = bookingService;
        this.userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? serviceType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var result = await bookingService.GetAllAsync(status, serviceType, from, to);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await bookingService.GetByIdAsync(id);
        if (result is null) return NotFound(new { message = "Booking not found." });
        return Ok(result);
    }
    
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(string customerId)
    {
        var result = await bookingService.GetByCustomerAsync(customerId);
        return Ok(result);
    }
    
    [HttpGet("staff/{staffId}")]
    public async Task<IActionResult> GetByStaff(string staffId)
    {
        var result = await bookingService.GetByStaffAsync(staffId);
        return Ok(result);
    }

    [HttpPost("customer/{customerId}")]
    public async Task<IActionResult> CreateByCustomer(
        string customerId, [FromBody] CreateBookingDto dto)
    {
        var customer = await userManager.FindByIdAsync(customerId);
        if (customer == null)
            return BadRequest(new { message = "Customer not found." });

        var isCustomer = await userManager.IsInRoleAsync(customer, "Customer");
        if (!isCustomer)
            return BadRequest(new { message = "User is not a customer." });

        var (success, message, booking) = await bookingService.CreateByCustomerAsync(customerId, dto);
        if (!success) return BadRequest(new { message });
        return CreatedAtAction(nameof(GetById), new { id = booking!.BookingId }, booking);
    }

    [HttpPost("staff/{staffId}")]
    public async Task<IActionResult> CreateByStaff(
        string staffId, [FromBody] CreateBookingDto dto)
    {
        var staff = await userManager.FindByIdAsync(staffId);
        if (staff == null)
            return BadRequest(new { message = "Staff not found." });

        var isStaff = await userManager.IsInRoleAsync(staff, "Staff")
                   || await userManager.IsInRoleAsync(staff, "Admin");
        if (!isStaff)
            return BadRequest(new { message = "User is not staff or admin." });

        var (success, message, booking) = await bookingService.CreateByStaffAsync(staffId, dto);
        if (!success) return BadRequest(new { message });
        return CreatedAtAction(nameof(GetById), new { id = booking!.BookingId }, booking);
    }
    
    [HttpPatch("{id:guid}/staff")]
    public async Task<IActionResult> UpdateByStaff(Guid id, [FromBody] UpdateBookingDto dto)
    {
        var (success, message, booking) = await bookingService.UpdateByStaffAsync(id, dto);
        if (!success) return BadRequest(new { message });
        return Ok(booking);
    }
    
    [HttpPatch("{id:guid}/customer/{customerId}")]
    public async Task<IActionResult> UpdateByCustomer(
        Guid id, string customerId, [FromBody] CustomerUpdateBookingDto dto)
    {
        var customer = await userManager.FindByIdAsync(customerId);
        if (customer == null)
            return BadRequest(new { message = "Customer not found." });

        var (success, message, booking) = await bookingService.UpdateByCustomerAsync(id, customerId, dto);
        if (!success) return BadRequest(new { message });
        return Ok(booking);
    }
}