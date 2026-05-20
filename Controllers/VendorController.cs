using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Implementations;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IVendorService vendorService;
        private readonly UserManager<User> userManager;

        public VendorController(ApplicationDbContext dbContext, IVendorService vendorService,UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.vendorService = vendorService;
            this.userManager = userManager;
        }

        [HttpPost("AddPartByVendor")]
        public async Task<IActionResult> AddPartByVendor(AddPartByVendorDTO vendorDTO)
        {
            var response = await vendorService.AddPart(vendorDTO);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }
        [HttpPost("UpdatePartByVendor/{partId}")]
        public async Task<IActionResult> UpdatePartByVendor(Guid partId, AddPartByVendorDTO vendorDTO)
        {
            var response = await vendorService.UpdatePart(partId, vendorDTO);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }
        [HttpDelete("DeletePartByVendor/{partId}")]
        public async Task<IActionResult> DeletePartByVendor(Guid partId)
        {
            var response = await vendorService.DeletePart(partId);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }

        [HttpPost("part-requests")]
        public async Task<IActionResult> AssignToPartRequest(
          [FromBody] AssignVendorToPartRequestModel model,
          [FromQuery] string assignedByUserId)
        {
            if (string.IsNullOrWhiteSpace(assignedByUserId))
                return BadRequest(new { message = "assignedByUserId query param is required." });

            var assigner = await userManager.FindByIdAsync(assignedByUserId);
            if (assigner == null)
                return BadRequest(new { message = "Assigning user not found." });

            var isAdminOrStaff = await userManager.IsInRoleAsync(assigner, "Admin")
                              || await userManager.IsInRoleAsync(assigner, "Staff");
            if (!isAdminOrStaff)
                return Forbid();

            var (success, message, assignment) =
                await vendorService.AssignVendorToPartRequestAsync(model, assignedByUserId);

            if (!success) return BadRequest(new { message });
            return Ok(assignment);
        }

        
        [HttpGet("part-requests/{partRequestId:guid}")]
        public async Task<IActionResult> GetByPartRequest(Guid partRequestId)
        {
            var result = await vendorService.GetAssignmentsByPartRequestAsync(partRequestId);
            return Ok(result);
        }

        

        
        [HttpPost("appointments")]
        public async Task<IActionResult> AssignToAppointment(
            [FromBody] AssignVendorToAppointmentModel model,
            [FromQuery] string assignedByUserId)
        {
            if (string.IsNullOrWhiteSpace(assignedByUserId))
                return BadRequest(new { message = "assignedByUserId query param is required." });

            var assigner = await userManager.FindByIdAsync(assignedByUserId);
            if (assigner == null)
                return BadRequest(new { message = "Assigning user not found." });

            var isAdminOrStaff = await userManager.IsInRoleAsync(assigner, "Admin")
                              || await userManager.IsInRoleAsync(assigner, "Staff");
            if (!isAdminOrStaff)
                return Forbid();

            var (success, message, assignment) =
                await vendorService.AssignVendorToAppointmentAsync(model, assignedByUserId);

            if (!success) return BadRequest(new { message });
            return Ok(assignment);
        }

        
        [HttpGet("appointments/{appointmentId:guid}")]
        public async Task<IActionResult> GetByAppointment(Guid appointmentId)
        {
            var result = await vendorService.GetAssignmentsByAppointmentAsync(appointmentId);
            return Ok(result);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await vendorService.GetAllAssignmentsAsync();
            return Ok(result);
        }

        
        [HttpGet("{assignmentId:guid}")]
        public async Task<IActionResult> GetById(Guid assignmentId)
        {
            var result = await vendorService.GetAssignmentByIdAsync(assignmentId);
            if (result == null) return NotFound(new { message = "Assignment not found." });
            return Ok(result);
        }

        
        [HttpDelete("{assignmentId:guid}")]
        public async Task<IActionResult> Delete(Guid assignmentId)
        {
            var (success, message) = await vendorService.DeleteAssignmentAsync(assignmentId);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }



        private async Task<Guid?> ResolveVendorIdAsync(string emailOrId)
        {
            
            var vendor = await dbContext.Vendors
                .FirstOrDefaultAsync(v => v.Email == emailOrId);

            if (vendor != null) return vendor.VendorId;

            if (Guid.TryParse(emailOrId, out var guid))
            {
                var byId = await dbContext.Vendors.FindAsync(guid);
                return byId?.VendorId;
            }

            return null;
        }

        private async Task<Guid?> ResolveVendorByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;

            var vendor = await dbContext.Vendors
                .FirstOrDefaultAsync(v => v.VendorId == guid);
            return vendor?.VendorId;
        }


        [HttpGet("assignments")]
        public async Task<IActionResult> GetMyAssignments([FromQuery] string vendorUserId)
        {
            if (string.IsNullOrWhiteSpace(vendorUserId))
                return BadRequest(new { message = "vendorUserId is required." });

            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null)
                return NotFound(new { message = $"No vendor found for: {vendorUserId}" });

            var result = await vendorService.GetMyAssignmentsAsync(vendorId.Value);
            return Ok(result);
        }


        [HttpGet("getVendorById")]
        public async Task<IActionResult> GetMyVendorId([FromQuery] string email)
        {
            var vendor = await dbContext.Vendors
                .FirstOrDefaultAsync(v => v.Email == email);

            if (vendor == null) return NotFound(new { message = "Vendor not found." });

            return Ok(new { vendorId = vendor.VendorId });
        }

        [HttpGet("assignments/{assignmentId:guid}")]
        public async Task<IActionResult> GetAssignment(Guid assignmentId, [FromQuery] string vendorUserId)
        {
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null) return Unauthorized(new { message = "Vendor user not found." });

            var result = await vendorService.GetMyAssignmentByIdAsync(assignmentId, vendorId.Value);
            if (result == null) return NotFound(new { message = "Assignment not found." });
            return Ok(result);
        }

        
        [HttpPatch("assignments/{assignmentId:guid}/respond")]
        public async Task<IActionResult> Respond(
            Guid assignmentId,
            [FromBody] VendorRespondModel model,
            [FromQuery] string vendorUserId)
        {
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null) return Unauthorized(new { message = "Vendor user not found." });

            var (success, message, assignment) =
                await vendorService.RespondToAssignmentAsync(assignmentId, vendorId.Value, model);

            if (!success) return BadRequest(new { message });
            return Ok(assignment);
        }

       
        [HttpPost("assignments/{assignmentId:guid}/invoices")]
        public async Task<IActionResult> GenerateInvoice(
            Guid assignmentId,
            [FromBody] CreateVendorInvoiceModel model,
            [FromQuery] string vendorUserId)
        {
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null) return Unauthorized(new { message = "Vendor user not found." });

            var (success, message, invoice) =
                await vendorService.GenerateInvoiceAsync(assignmentId, vendorId.Value, model);

            if (!success) return BadRequest(new { message });
            return Ok(invoice);
        }


        [HttpGet("invoices")]
        public async Task<IActionResult> GetMyInvoices([FromQuery] string vendorUserId)
        {
            if (string.IsNullOrWhiteSpace(vendorUserId))
                return BadRequest(new { message = "vendorUserId is required." });

            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null)
                return NotFound(new { message = $"No vendor found for: {vendorUserId}" });

            var result = await vendorService.GetMyInvoicesAsync(vendorId.Value);
            return Ok(result);
        }


        [HttpGet("invoices/{invoiceId:guid}")]
        public async Task<IActionResult> GetInvoice(Guid invoiceId, [FromQuery] string vendorUserId)
        {
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null) return Unauthorized(new { message = "Vendor user not found." });

            var result = await vendorService.GetMyInvoiceByIdAsync(invoiceId, vendorId.Value);
            if (result == null) return NotFound(new { message = "Invoice not found." });
            return Ok(result);
        }


        // ─────────────────────────────────────────────────────────────────────────────
        // ADD THIS METHOD to Controllers/VendorController.cs
        // inside the VendorController class, after the existing GetInvoice endpoint
        // ─────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// POST /api/Vendor/invoices/{invoiceId}/send-email?vendorUserId=vendor@email.com
        /// Vendor sends their invoice to the customer via email.
        /// </summary>
        [HttpPost("invoices/{invoiceId:guid}/send-email")]
        public async Task<IActionResult> SendVendorInvoiceEmail(
            Guid invoiceId,
            [FromQuery] string vendorUserId,
            [FromServices] IEmailService emailService)
        {
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null)
                return Unauthorized(new { message = "Vendor user not found." });

            var invoice = await dbContext.VendorInvoices
                .Where(vi => vi.VendorInvoiceId == invoiceId && vi.VendorId == vendorId)
                .Include(vi => vi.Vendor)
                .Include(vi => vi.Customer)
                .Include(vi => vi.Items)
                .Include(vi => vi.VendorAssignment)
                    .ThenInclude(va => va.PartRequest)
                .Include(vi => vi.VendorAssignment)
                    .ThenInclude(va => va.Appointment)
                .FirstOrDefaultAsync();

            if (invoice == null)
                return NotFound(new { message = "Invoice not found." });

            var customerEmail = invoice.Customer?.Email;
            if (string.IsNullOrWhiteSpace(customerEmail))
                return BadRequest(new { message = "Customer does not have an email address on file." });

            var customerName = $"{invoice.Customer?.FirstName} {invoice.Customer?.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(customerName))
                customerName = invoice.Customer?.UserName ?? "Valued Customer";

            var subject = invoice.VendorAssignment?.PartRequest != null
                ? $"Part Request Invoice {invoice.InvoiceNumber} – {invoice.Vendor?.Name}"
                : $"Service Invoice {invoice.InvoiceNumber} – {invoice.Vendor?.Name}";

            var subjectLine = invoice.VendorAssignment?.PartRequest?.PartName
                           ?? invoice.VendorAssignment?.Appointment?.ServiceType
                           ?? "Service";

            var itemRows = string.Join("", invoice.Items.Select(i => $@"
        <tr>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;'>{i.Description}</td>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;text-align:center;'>{i.ItemType}</td>
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
            Vendor Service Invoice
          </p>
        </div>
        <div style='background:#eff6ff;padding:20px 36px;border-bottom:1px solid #dbeafe;display:flex;justify-content:space-between;align-items:center;'>
          <div>
            <p style='margin:0;color:#1d4ed8;font-size:11px;font-weight:800;letter-spacing:.12em;text-transform:uppercase;'>Invoice</p>
            <p style='margin:4px 0 0;color:#1e3a8a;font-size:22px;font-weight:900;'>{invoice.InvoiceNumber}</p>
          </div>
          <div style='text-align:right;'>
            <p style='margin:0;color:#6b7280;font-size:12px;'>Issued</p>
            <p style='margin:4px 0 0;color:#111827;font-weight:700;font-size:14px;'>{invoice.IssuedAt:dd MMM yyyy}</p>
          </div>
        </div>
        <div style='padding:28px 36px 0;'>
          <p style='margin:0;color:#374151;font-size:15px;'>Hi <strong>{customerName}</strong>,</p>
          <p style='margin:8px 0 0;color:#6b7280;font-size:14px;line-height:1.6;'>
            Please find your invoice for <strong>{subjectLine}</strong> from <strong>{invoice.Vendor?.Name}</strong>.
          </p>
        </div>
        <div style='padding:24px 36px;'>
          <table style='width:100%;border-collapse:collapse;font-size:14px;'>
            <thead>
              <tr style='background:#f8fafc;'>
                <th style='padding:10px 14px;text-align:left;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Description</th>
                <th style='padding:10px 14px;text-align:center;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Type</th>
                <th style='padding:10px 14px;text-align:center;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Qty</th>
                <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Unit Price</th>
                <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Total</th>
              </tr>
            </thead>
            <tbody>
              {itemRows}
              <tr style='background:#f8fafc;'>
                <td colspan='4' style='padding:14px;text-align:right;color:#374151;font-weight:700;font-size:15px;'>Grand Total</td>
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
            Thank you for choosing <strong>VMSKTM</strong>. Contact us if you have any questions.
          </p>
        </div>
      </div>
    </body>
    </html>";

            try
            {
                await emailService.SendEmailAsync(customerEmail, customerName, subject, html);
                return Ok(new { message = "Invoice email sent to customer successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to send email: {ex.Message}" });
            }
        }

      

        [HttpPost("admin/invoices/{invoiceId:guid}/send-email")]
        public async Task<IActionResult> AdminSendVendorInvoiceEmail(
            Guid invoiceId,
            [FromServices] IEmailService emailService)
        {
            var invoice = await dbContext.VendorInvoices
                .Where(vi => vi.VendorInvoiceId == invoiceId)
                .Include(vi => vi.Vendor)
                .Include(vi => vi.Customer)
                .Include(vi => vi.Items)
                .Include(vi => vi.VendorAssignment)
                    .ThenInclude(va => va.PartRequest)
                .Include(vi => vi.VendorAssignment)
                    .ThenInclude(va => va.Appointment)
                .FirstOrDefaultAsync();

            if (invoice == null)
                return NotFound(new { message = "Vendor invoice not found." });

            
            var vendorRecord = invoice.Vendor;
            var toEmail = vendorRecord?.Email;
            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest(new { message = "Vendor does not have an email address on file." });

            var toName = vendorRecord?.Name ?? "Vendor";
            var subject = $"Invoice Copy {invoice.InvoiceNumber} – VMSKTM Admin";
            var subjectLine = invoice.VendorAssignment?.PartRequest?.PartName
                           ?? invoice.VendorAssignment?.Appointment?.ServiceType
                           ?? "Service";

            var customerName = $"{invoice.Customer?.FirstName} {invoice.Customer?.LastName}".Trim()
                            is { Length: > 0 } n ? n : (invoice.Customer?.UserName ?? "Customer");

            var itemRows = string.Join("", invoice.Items.Select(i => $@"
        <tr>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;'>{i.Description}</td>
            <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;text-align:center;'>{i.ItemType}</td>
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
            Admin — Invoice Record Copy
          </p>
        </div>
        <div style='background:#eff6ff;padding:20px 36px;border-bottom:1px solid #dbeafe;'>
          <p style='margin:0;color:#1d4ed8;font-size:11px;font-weight:800;letter-spacing:.12em;text-transform:uppercase;'>Invoice Reference</p>
          <p style='margin:4px 0 0;color:#1e3a8a;font-size:22px;font-weight:900;'>{invoice.InvoiceNumber}</p>
          <p style='margin:6px 0 0;color:#6b7280;font-size:13px;'>Customer: <strong>{customerName}</strong> &nbsp;|&nbsp; Issued: <strong>{invoice.IssuedAt:dd MMM yyyy}</strong></p>
        </div>
        <div style='padding:28px 36px 0;'>
          <p style='margin:0;color:#374151;font-size:15px;'>Hi <strong>{toName}</strong>,</p>
          <p style='margin:8px 0 0;color:#6b7280;font-size:14px;line-height:1.6;'>
            This is an admin-issued copy of the invoice for <strong>{subjectLine}</strong> provided to <strong>{customerName}</strong>. Please keep it for your records.
          </p>
        </div>
        <div style='padding:24px 36px;'>
          <table style='width:100%;border-collapse:collapse;font-size:14px;'>
            <thead>
              <tr style='background:#f8fafc;'>
                <th style='padding:10px 14px;text-align:left;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Description</th>
                <th style='padding:10px 14px;text-align:center;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Type</th>
                <th style='padding:10px 14px;text-align:center;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Qty</th>
                <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Unit Price</th>
                <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Total</th>
              </tr>
            </thead>
            <tbody>
              {itemRows}
              <tr style='background:#f8fafc;'>
                <td colspan='4' style='padding:14px;text-align:right;color:#374151;font-weight:700;font-size:15px;'>Grand Total</td>
                <td style='padding:14px;text-align:right;color:#1d4ed8;font-weight:900;font-size:20px;'>Rs. {invoice.TotalAmount:N2}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div style='background:#f8fafc;padding:24px 36px;border-top:1px solid #e5e7eb;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;line-height:1.6;'>
            Sent by the VMSKTM Admin team. Contact the system admin if you have any discrepancies.
          </p>
        </div>
      </div>
    </body>
    </html>";

            try
            {
                await emailService.SendEmailAsync(toEmail, toName, subject, html);
                return Ok(new { message = $"Invoice emailed to vendor {toName} successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to send email: {ex.Message}" });
            }
        }

    }
}