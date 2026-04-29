using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly ISaleService saleService;

    public SaleController(UserManager<User> userManager, ISaleService saleService)
    {
        this.userManager = userManager;
        this.saleService = saleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto createSaleDto)
    {
        
        if (string.IsNullOrWhiteSpace(createSaleDto.StaffId))
            return BadRequest(new { message = "StaffId is required." });

        
        var staff = await userManager.FindByIdAsync(createSaleDto.StaffId);
        if (staff == null)
            return BadRequest(new { message = "Staff user not found." });

        var isStaff = await userManager.IsInRoleAsync(staff, "Staff");
        var isAdmin = await userManager.IsInRoleAsync(staff, "Admin");
        if (!isStaff && !isAdmin)
            return BadRequest(new { message = "User is not authorized to create sales." });

        var (success, message, invoice) = await saleService.CreateSale(createSaleDto, createSaleDto.StaffId);
        if (!success)
            return BadRequest(new { message });

        return Ok(invoice);
    }

    [HttpGet("{saleId:guid}")]
    public async Task<IActionResult> GetInvoiceById(Guid saleId)
    {
        var (success, message, invoice) = await saleService.GetInvoiceById(saleId);
        if (!success)
            return NotFound(new { message });

        return Ok(invoice);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllInvoices()
    {
        var (_, _, invoices) = await saleService.GetAllInvoices();
        return Ok(invoices);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetInvoicesByCustomer(string customerId)
    {
        var (success, message, invoices) = await saleService.GetInvoicesByCustomer(customerId);
        if (!success)
            return NotFound(new { message });

        return Ok(invoices);
    }
}