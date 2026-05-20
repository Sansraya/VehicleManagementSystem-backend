using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
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
        private readonly IEmailService emailService;
        private readonly ApplicationDbContext dbContext;
        

        public AdminController(
            IStaffService staffService,
            IVendorService vendorService,
            IPartsService partsService,
            IInvoiceService invoiceService,
            IEmailService emailService,
            ApplicationDbContext dbContext)
            
        {
            this.staffService = staffService;
            this.vendorService = vendorService;
            this.partsService = partsService;
            this.invoiceService = invoiceService;
            this.emailService = emailService;
            this.dbContext = dbContext;
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


        [HttpPost("invoices/{id}/send-email")]
        public async Task<IActionResult> SendPurchaseInvoiceEmail(Guid id)
        {
            
            var invoice = await dbContext.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .FirstOrDefaultAsync(i => i.PurchaseInvoiceId == id);

            if (invoice == null)
                return NotFound(new { message = "Invoice not found." });

            var toEmail = invoice.Vendor?.Email;
            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest(new { message = "Vendor does not have an email address on file." });

            var toName = invoice.Vendor!.Name;
            var subject = $"Purchase Order {invoice.InvoiceNumber} – VMSKTM";

            var itemRows = string.Join("", invoice.Items.Select(i => $@"
        <tr>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;'>{i.Part?.Name ?? "—"}</td>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;text-align:center;'>{i.Quantity}</td>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;text-align:right;'>Rs. {i.UnitPrice:N2}</td>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#111827;font-weight:600;text-align:right;'>Rs. {i.SubTotal:N2}</td>
        </tr>"));

            var html = $@"
    <html lang='en'>
    <head><meta charset='UTF-8'/></head>
    <body style='margin:0;padding:0;background:#f9fafb;font-family:Inter,Arial,sans-serif;'>
      <div style='max-width:620px;margin:40px auto;background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.07);'>

        <div style='background:#0f172a;padding:32px 36px;'>
          <h1 style='margin:0;color:#fff;font-size:28px;font-weight:900;letter-spacing:-1px;'>
            VMS<span style='color:#3b82f6;'>KTM</span>
          </h1>
          <p style='margin:6px 0 0;color:#94a3b8;font-size:13px;font-weight:600;letter-spacing:.05em;text-transform:uppercase;'>
            Purchase Order
          </p>
        </div>

        <div style='background:#eff6ff;padding:20px 36px;border-bottom:1px solid #dbeafe;display:flex;justify-content:space-between;align-items:center;'>
          <div>
            <p style='margin:0;color:#1d4ed8;font-size:11px;font-weight:800;letter-spacing:.12em;text-transform:uppercase;'>PO Number</p>
            <p style='margin:4px 0 0;color:#1e3a8a;font-size:22px;font-weight:900;'>{invoice.InvoiceNumber}</p>
          </div>
          <div style='text-align:right;'>
            <p style='margin:0;color:#6b7280;font-size:12px;'>Date</p>
            <p style='margin:4px 0 0;color:#111827;font-weight:700;font-size:14px;'>{invoice.InvoiceDate:dd MMM yyyy}</p>
            <p style='margin:4px 0 0;font-size:11px;font-weight:700;padding:3px 10px;border-radius:20px;display:inline-block;
                {(invoice.Status == "Received" ? "background:#d1fae5;color:#065f46;" : invoice.Status == "Cancelled" ? "background:#fee2e2;color:#991b1b;" : "background:#ffedd5;color:#9a3412;")}'
            >{invoice.Status}</p>
          </div>
        </div>

        <div style='padding:28px 36px 0;'>
          <p style='margin:0;color:#374151;font-size:15px;'>Hi <strong>{toName}</strong>,</p>
          <p style='margin:8px 0 0;color:#6b7280;font-size:14px;line-height:1.6;'>
            Please find the details of your purchase order below. Contact us if you have any questions.
          </p>
        </div>

        <div style='padding:24px 36px;'>
          <table style='width:100%;border-collapse:collapse;font-size:14px;'>
            <thead>
              <tr style='background:#f8fafc;'>
                <th style='padding:10px 14px;text-align:left;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Part</th>
                <th style='padding:10px 14px;text-align:center;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Qty</th>
                <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Unit Price</th>
                <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Total</th>
              </tr>
            </thead>
            <tbody>
              {itemRows}
              <tr style='background:#f8fafc;'>
                <td colspan='3' style='padding:14px;text-align:right;color:#374151;font-weight:700;font-size:15px;'>Order Total</td>
                <td style='padding:14px;text-align:right;color:#1d4ed8;font-weight:900;font-size:20px;'>Rs. {invoice.TotalAmount:N2}</td>
              </tr>
            </tbody>
          </table>
        </div>

        {(string.IsNullOrWhiteSpace(invoice.Notes) ? "" : $@"
        <div style='margin:0 36px 20px;background:#f8fafc;border-radius:10px;padding:14px 18px;'>
          <p style='margin:0;color:#6b7280;font-size:13px;'><strong>Notes:</strong> {invoice.Notes}</p>
        </div>")}

        <div style='background:#f8fafc;padding:24px 36px;border-top:1px solid #e5e7eb;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;line-height:1.6;'>
            Thank you for your partnership with <strong>VMSKTM</strong>. Please process this order at your earliest convenience.
          </p>
        </div>

      </div>
    </body>
    </html>";

            try
            {
                await emailService.SendEmailAsync(toEmail, toName, subject, html);
                return Ok(new { message = $"Purchase invoice emailed to {toName} successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to send email: {ex.Message}" });
            }
        }

    }
}