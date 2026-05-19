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



        private async Task<Guid?> ResolveVendorIdAsync(string email)
        {

            var vendor = await dbContext.Vendors
                .FirstOrDefaultAsync(v => v.Email == email);
            return vendor?.VendorId;
        }


        [HttpGet("assignments")]
        public async Task<IActionResult> GetMyAssignments([FromQuery] string vendorUserId)
        {
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null) return Unauthorized(new { message = "Vendor user not found." });

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
            var vendorId = await ResolveVendorIdAsync(vendorUserId);
            if (vendorId == null) return Unauthorized(new { message = "Vendor user not found." });

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

    }
}