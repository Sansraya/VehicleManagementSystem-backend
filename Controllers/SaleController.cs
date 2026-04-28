using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Services.Implementation;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SaleController(ISaleService saleService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto createSaleDto)
    {
        var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (staffId == null)
            return Unauthorized(new { message = "Staff not authenticated." });

        var (success, message, invoice) = await saleService.CreateSale(createSaleDto, staffId);
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