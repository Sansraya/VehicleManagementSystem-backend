using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IStaffService staffService;
        private readonly IVendorService vendorService;
        private readonly IPartsService partsService;
        private readonly IInvoiceService invoiceService;

        public AdminController(
            IStaffService staffService,
            IVendorService vendorService,
            IPartsService partsService,
            IInvoiceService invoiceService)
        {
            this.staffService = staffService;
            this.vendorService = vendorService;
            this.partsService = partsService;
            this.invoiceService = invoiceService;
        }

      

        [HttpPost("staff/register")]
        public async Task<IActionResult> RegisterStaff([FromForm] StaffRegisterModel model)
        {
            var (success, message) = await staffService.RegisterStaffAsync(model);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpGet("staff")]
        public async Task<IActionResult> GetAllStaff()
        {
            var result = await staffService.GetAllStaffAsync();
            return Ok(result);
        }

        [HttpGet("staff/{id}")]
        public async Task<IActionResult> GetStaff(string id)
        {
            var result = await staffService.GetStaffByIdAsync(id);
            if (result == null) return NotFound(new { message = "Staff not found" });
            return Ok(result);
        }

        [HttpPut("staff/{id}")]
        public async Task<IActionResult> UpdateStaff(string id, [FromForm] UserDto model)
        {
            var (success, message) = await staffService.UpdateStaffAsync(id, model);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpDelete("staff/{id}")]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            var (success, message) = await staffService.DeleteStaffAsync(id);
            if (!success) return NotFound(new { message });
            return Ok(new { message });
        }

       

        [HttpPost("vendors")]
        public async Task<IActionResult> CreateVendor([FromForm] VendorModel model)
        {
            var (success, message, vendorId) = await vendorService.CreateVendorAsync(model);
            if (!success) return BadRequest(new { message });
            return Ok(new { message, vendorId });
        }

        [HttpGet("vendors")]
        public async Task<IActionResult> GetVendors()
        {
            var result = await vendorService.GetAllVendorsAsync();
            return Ok(result);
        }

        [HttpGet("vendors/{id}")]
        public async Task<IActionResult> GetVendor(Guid id)
        {
            var result = await vendorService.GetVendorByIdAsync(id);
            if (result == null) return NotFound(new { message = "Vendor not found" });
            return Ok(result);
        }

        [HttpPut("vendors/{id}")]
        public async Task<IActionResult> UpdateVendor(Guid id, [FromForm] VendorModel model)
        {
            var (success, message) = await vendorService.UpdateVendorAsync(id, model);
            if (!success) return NotFound(new { message });
            return Ok(new { message });
        }

        [HttpDelete("vendors/{id}")]
        public async Task<IActionResult> DeleteVendor(Guid id)
        {
            var (success, message) = await vendorService.DeleteVendorAsync(id);
            if (!success) return NotFound(new { message });
            return Ok(new { message });
        }

       

        [HttpPost("parts")]
        public async Task<IActionResult> CreatePart([FromForm] PartModel model)
        {
            var (success, message, partsId) = await partsService.CreatePartAsync(model);
            if (!success) return BadRequest(new { message });
            return Ok(new { message, partsId });
        }

        [HttpGet("parts")]
        public async Task<IActionResult> GetParts()
        {
            var result = await partsService.GetAllPartsAsync();
            return Ok(result);
        }

        [HttpGet("parts/{id}")]
        public async Task<IActionResult> GetPart(Guid id)
        {
            var result = await partsService.GetPartByIdAsync(id);
            if (result == null) return NotFound(new { message = "Part not found" });
            return Ok(result);
        }

        [HttpPut("parts/{id}")]
        public async Task<IActionResult> UpdatePart(Guid id, [FromForm] PartModel model)
        {
            var (success, message) = await partsService.UpdatePartAsync(id, model);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpDelete("parts/{id}")]
        public async Task<IActionResult> DeletePart(Guid id)
        {
            var (success, message) = await partsService.DeletePartAsync(id);
            if (!success) return NotFound(new { message });
            return Ok(new { message });
        }

        

        [HttpPost("invoices")]
        public async Task<IActionResult> CreateInvoice([FromBody] PurchaseInvoiceModel model)
        {
            var (success, message, invoiceNumber, totalAmount) = await invoiceService.CreateInvoiceAsync(model);
            if (!success) return BadRequest(new { message });
            return Ok(new { message, invoiceNumber, totalAmount });
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            var result = await invoiceService.GetAllInvoicesAsync();
            return Ok(result);
        }

        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var result = await invoiceService.GetInvoiceByIdAsync(id);
            if (result == null) return NotFound(new { message = "Invoice not found" });
            return Ok(result);
        }

        [HttpPatch("invoices/{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(Guid id, [FromForm] string status)
        {
            var (success, message) = await invoiceService.UpdateInvoiceStatusAsync(id, status);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }
    }
}