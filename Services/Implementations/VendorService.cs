using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class VendorsService : IVendorService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;

        public VendorsService(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<(bool Success, string Message, Guid? VendorId)> CreateVendorAsync(VendorModel model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return (false, "Vendor did not create currently!", null);

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var vendor = new Vendor
            {
                VendorId = Guid.NewGuid(),
                Name = model.Username,
                Email = model.Email,
                Phone = model.Phone ?? "",
                Address = model.Address ?? "",
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Vendors.AddAsync(vendor);
            await dbContext.SaveChangesAsync();
            return (true, "Vendor created", vendor.VendorId);
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            return await dbContext.Vendors
                .Select(v => new VendorDto
                {
                    VendorId = v.VendorId,
                    Name = v.Name,
                    ContactPerson = v.ContactPerson,
                    Email = v.Email,
                    Phone = v.Phone,
                    Address = v.Address,
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<VendorDto?> GetVendorByIdAsync(Guid id)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return null;

            return new VendorDto
            {
                VendorId = vendor.VendorId,
                Name = vendor.Name,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                Phone = vendor.Phone,
                Address = vendor.Address,
                CreatedAt = vendor.CreatedAt
            };
        }

        public async Task<(bool Success, string Message)> UpdateVendorAsync(Guid id, VendorModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return (false, "Vendor not found");

            vendor.Name = model.Username;
            vendor.ContactPerson = model.Phone;
            vendor.Email = model.Email;
            vendor.Phone = model.Phone;
            vendor.Address = model.Address;

            await dbContext.SaveChangesAsync();
            return (true, "Vendor updated");
        }

        public async Task<(bool Success, string Message)> DeleteVendorAsync(Guid id)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return (false, "Vendor not found");

            dbContext.Vendors.Remove(vendor);
            await dbContext.SaveChangesAsync();
            return (true, "Vendor deleted");
        }


        public async Task<VendorResponse> AddPart(AddPartByVendorDTO vendorDTO)
        {
            var vendor = await dbContext.Vendors.FindAsync(vendorDTO.Vendorid);
            if (vendor == null)
            {
                return new VendorResponse
                {
                    Success = false,
                    Message = "Vendor not found."
                };
            }

            var part = new Parts
            {
                VendorId = vendor.VendorId,
                Name = vendorDTO.Name,
                PartCode = vendorDTO.PartCode,
                Category = vendorDTO.Category,
                Description = vendorDTO.Description,
                Price = vendorDTO.Price,
                StockQuantity = vendorDTO.StockQuantity,
                CreatedAt = vendorDTO.CreatedAt,
                UpdatedAt = vendorDTO.UpdatedAt
            };

            await dbContext.Parts.AddAsync(part);
            await dbContext.SaveChangesAsync();
            return new VendorResponse
            {
                Success = true,
                Message = "Part added successfully."
            };

        }
        public async Task<VendorResponse> UpdatePart(Guid partId, AddPartByVendorDTO vendorDTO)
        {
            var part = await dbContext.Parts.FindAsync(partId);
            if (part == null)
            {
                return new VendorResponse
                {
                    Success = false,
                    Message = "Part not found."
                };
            }
            part.Name = vendorDTO.Name;
            part.PartCode = vendorDTO.PartCode;
            part.Category = vendorDTO.Category;
            part.Description = vendorDTO.Description;
            part.Price = vendorDTO.Price;
            part.StockQuantity = vendorDTO.StockQuantity;
            part.UpdatedAt = DateTime.UtcNow;
            dbContext.Parts.Update(part);
            await dbContext.SaveChangesAsync();
            return new VendorResponse
            {
                Success = true,
                Message = "Part updated successfully."
            };
        }

        public async Task<VendorResponse> DeletePart(Guid partId)
        {
            var part = await dbContext.Parts.FindAsync(partId);
            if (part == null)
            {
                return new VendorResponse
                {
                    Success = false,
                    Message = "Part not found."
                };
            }
            dbContext.Parts.Remove(part);
            await dbContext.SaveChangesAsync();
            return new VendorResponse
            {
                Success = true,
                Message = "Part deleted successfully."
            };
        }


        public async Task<(bool Success, string Message, VendorAssignmentDto? Assignment)>
         AssignVendorToPartRequestAsync(AssignVendorToPartRequestModel model, string assignedByUserId)
        {
            var partRequest = await dbContext.PartRequests
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.PartRequestId == model.PartRequestId);
            if (partRequest == null)
                return (false, "Part request not found.", null);

            var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
            if (vendor == null)
                return (false, "Vendor not found.", null);

            var assignedBy = await dbContext.Users.FindAsync(assignedByUserId);
            if (assignedBy == null)
                return (false, "Assigning user not found.", null);


            var duplicate = await dbContext.VendorAssignments
                .AnyAsync(va => va.PartRequestId == model.PartRequestId
                             && va.VendorId == model.VendorId
                             && va.Status == "Pending");
            if (duplicate)
                return (false, "This vendor already has a pending assignment for this part request.", null);

            var assignment = new VendorAssignment
            {
                PartRequestId = model.PartRequestId,
                AppointmentId = null,
                VendorId = model.VendorId,
                Vendor = vendor,
                AssignedByUserId = assignedByUserId,
                Notes = model.Notes,
                AssignedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            dbContext.VendorAssignments.Add(assignment);
            await dbContext.SaveChangesAsync();

            return (true, "Vendor assigned to part request.", MapToDto(assignment, partRequest.Customer));
        }



        public async Task<(bool Success, string Message, VendorAssignmentDto? Assignment)>
            AssignVendorToAppointmentAsync(AssignVendorToAppointmentModel model, string assignedByUserId)
        {
            var appointment = await dbContext.Appointments
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);
            if (appointment == null)
                return (false, "Appointment not found.", null);

            var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
            if (vendor == null)
                return (false, "Vendor not found.", null);

            var assignedBy = await dbContext.Users.FindAsync(assignedByUserId);
            if (assignedBy == null)
                return (false, "Assigning user not found.", null);

            var duplicate = await dbContext.VendorAssignments
                .AnyAsync(va => va.AppointmentId == model.AppointmentId
                             && va.VendorId == model.VendorId
                             && va.Status == "Pending");
            if (duplicate)
                return (false, "This vendor already has a pending assignment for this appointment.", null);

            var assignment = new VendorAssignment
            {
                PartRequestId = null,
                AppointmentId = model.AppointmentId,
                VendorId = model.VendorId,
                Vendor = vendor,
                AssignedByUserId = assignedByUserId,
                Notes = model.Notes,
                AssignedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            dbContext.VendorAssignments.Add(assignment);
            await dbContext.SaveChangesAsync();

            return (true, "Vendor assigned to appointment.", MapToDto(assignment, appointment.Customer));
        }



        public async Task<IEnumerable<VendorAssignmentDto>> GetAllAssignmentsAsync()
        {
            var assignments = await dbContext.VendorAssignments
                .Include(va => va.Vendor)
                .Include(va => va.AssignedBy)
                .Include(va => va.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(va => va.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(va => va.VendorInvoice)
                .OrderByDescending(va => va.AssignedAt)
                .ToListAsync();

            return assignments.Select(va => MapToDto(va, ResolveCustomer(va)));
        }

        public async Task<IEnumerable<VendorAssignmentDto>> GetAssignmentsByPartRequestAsync(Guid partRequestId)
        {
            var assignments = await dbContext.VendorAssignments
                .Where(va => va.PartRequestId == partRequestId)
                .Include(va => va.Vendor)
                .Include(va => va.AssignedBy)
                .Include(va => va.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(va => va.VendorInvoice)
                .OrderByDescending(va => va.AssignedAt)
                .ToListAsync();

            return assignments.Select(va => MapToDto(va, va.PartRequest?.Customer));
        }

        public async Task<IEnumerable<VendorAssignmentDto>> GetAssignmentsByAppointmentAsync(Guid appointmentId)
        {
            var assignments = await dbContext.VendorAssignments
                .Where(va => va.AppointmentId == appointmentId)
                .Include(va => va.Vendor)
                .Include(va => va.AssignedBy)
                .Include(va => va.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(va => va.VendorInvoice)
                .OrderByDescending(va => va.AssignedAt)
                .ToListAsync();

            return assignments.Select(va => MapToDto(va, va.Appointment?.Customer));
        }

        public async Task<VendorAssignmentDto?> GetAssignmentByIdAsync(Guid assignmentId)
        {
            var va = await dbContext.VendorAssignments
                .Include(x => x.Vendor)
                .Include(x => x.AssignedBy)
                .Include(x => x.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(x => x.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(x => x.VendorInvoice)
                .FirstOrDefaultAsync(x => x.VendorAssignmentId == assignmentId);

            return va == null ? null : MapToDto(va, ResolveCustomer(va));
        }



        public async Task<(bool Success, string Message)> DeleteAssignmentAsync(Guid assignmentId)
        {
            var va = await dbContext.VendorAssignments.FindAsync(assignmentId);
            if (va == null) return (false, "Assignment not found.");
            if (va.Status == "Accepted")
                return (false, "Cannot delete an accepted assignment. Deny it first or cancel the invoice.");

            dbContext.VendorAssignments.Remove(va);
            await dbContext.SaveChangesAsync();
            return (true, "Assignment deleted.");
        }



        private static User? ResolveCustomer(VendorAssignment va)
        {
            if (va.PartRequest != null) return va.PartRequest.Customer;
            if (va.Appointment != null) return va.Appointment.Customer;
            return null;
        }

        private static VendorAssignmentDto MapToDto(VendorAssignment va, User? customer)
        {
            var dto = new VendorAssignmentDto
            {
                VendorAssignmentId = va.VendorAssignmentId,
                VendorId = va.VendorId,
                VendorName = va.Vendor?.Name,
                PartRequestId = va.PartRequestId,
                PartRequestPartName = va.PartRequest?.PartName,
                AppointmentId = va.AppointmentId,
                AppointmentServiceType = va.Appointment?.ServiceType,
                AssignedByUserId = va.AssignedByUserId,
                AssignedByName = va.AssignedBy == null
                    ? null
                    : $"{va.AssignedBy.FirstName} {va.AssignedBy.LastName}".Trim(),
                AssignedAt = va.AssignedAt,
                Status = va.Status,
                Notes = va.Notes,
                RespondedAt = va.RespondedAt,
                VendorNotes = va.VendorNotes,
                VendorInvoiceId = va.VendorInvoice?.VendorInvoiceId,
                InvoiceNumber = va.VendorInvoice?.InvoiceNumber,
                CustomerId = customer?.Id,
                CustomerName = customer == null
                    ? null
                    : $"{customer.FirstName} {customer.LastName}".Trim() is { Length: > 0 } n ? n : customer.UserName,
            };
            return dto;
        }






        public async Task<IEnumerable<VendorAssignmentDto>> GetMyAssignmentsAsync(Guid vendorId)
        {
            var assignments = await dbContext.VendorAssignments
                .Where(va => va.VendorId == vendorId)
                .Include(va => va.Vendor)
                .Include(va => va.AssignedBy)
                .Include(va => va.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(va => va.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(va => va.VendorInvoice)
                .OrderByDescending(va => va.AssignedAt)
                .ToListAsync();

            return assignments.Select(MapAssignmentToDto);
        }

        public async Task<VendorAssignmentDto?> GetMyAssignmentByIdAsync(Guid assignmentId, Guid vendorId)
        {
            var va = await dbContext.VendorAssignments
                .Where(x => x.VendorAssignmentId == assignmentId && x.VendorId == vendorId)
                .Include(x => x.Vendor)
                .Include(x => x.AssignedBy)
                .Include(x => x.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(x => x.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(x => x.VendorInvoice)
                .FirstOrDefaultAsync();

            return va == null ? null : MapAssignmentToDto(va);
        }



        public async Task<(bool Success, string Message, VendorAssignmentDto? Assignment)>
            RespondToAssignmentAsync(Guid assignmentId, Guid vendorId, VendorRespondModel model)
        {
            var allowed = new[] { "Accepted", "Denied" };
            if (!allowed.Contains(model.Decision))
                return (false, "Decision must be 'Accepted' or 'Denied'.", null);

            var va = await dbContext.VendorAssignments
                .Include(x => x.Vendor)
                .Include(x => x.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(x => x.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(x => x.VendorInvoice)
                .FirstOrDefaultAsync(x => x.VendorAssignmentId == assignmentId && x.VendorId == vendorId);

            if (va == null) return (false, "Assignment not found.", null);
            if (va.Status != "Pending")
                return (false, $"Assignment is already '{va.Status}' and cannot be changed.", null);

            va.Status = model.Decision;
            va.VendorNotes = model.VendorNotes;
            va.RespondedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return (true, $"Assignment {model.Decision.ToLower()}.", MapAssignmentToDto(va));
        }



        public async Task<(bool Success, string Message, VendorInvoiceDto? Invoice)>
            GenerateInvoiceAsync(Guid assignmentId, Guid vendorId, CreateVendorInvoiceModel model)
        {
            if (model.Items == null || !model.Items.Any())
                return (false, "At least one item is required.", null);

            var va = await dbContext.VendorAssignments
                .Include(x => x.Vendor)
                .Include(x => x.PartRequest).ThenInclude(pr => pr != null ? pr.Customer : null)
                .Include(x => x.Appointment).ThenInclude(a => a != null ? a.Customer : null)
                .Include(x => x.VendorInvoice)
                .FirstOrDefaultAsync(x => x.VendorAssignmentId == assignmentId && x.VendorId == vendorId);

            if (va == null) return (false, "Assignment not found.", null);
            if (va.Status != "Accepted")
                return (false, "You can only generate an invoice for an accepted assignment.", null);
            if (va.VendorInvoice != null)
                return (false, "An invoice already exists for this assignment.", null);


            var customer = va.PartRequest?.Customer ?? va.Appointment?.Customer;
            if (customer == null) return (false, "Could not resolve customer for this assignment.", null);


            foreach (var item in model.Items)
            {
                if (item.Quantity <= 0)
                    return (false, $"Quantity for '{item.Description}' must be > 0.", null);
                if (item.UnitPrice <= 0)
                    return (false, $"Unit price for '{item.Description}' must be > 0.", null);
                if (!new[] { "Part", "Service" }.Contains(item.ItemType))
                    return (false, $"ItemType must be 'Part' or 'Service' (got '{item.ItemType}').", null);
            }

            var invoiceItems = model.Items.Select(i => new VendorInvoiceItem
            {
                VendorInvoiceItemId = Guid.NewGuid(),
                Description = i.Description,
                ItemType = i.ItemType,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var totalAmount = invoiceItems.Sum(i => i.Quantity * i.UnitPrice);

            var invoice = new VendorInvoice
            {
                VendorInvoiceId = Guid.NewGuid(),
                InvoiceNumber = $"VINV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                VendorAssignmentId = va.VendorAssignmentId,
                VendorAssignment = va,
                VendorId = vendorId,
                Vendor = va.Vendor!,
                CustomerId = customer.Id,
                Customer = customer,
                Notes = model.Notes,
                TotalAmount = totalAmount,
                Status = "Issued",
                IssuedAt = DateTime.UtcNow,
                Items = invoiceItems
            };

            dbContext.VendorInvoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            return (true, "Invoice generated successfully.", MapInvoiceToDto(invoice, va));
        }



        public async Task<IEnumerable<VendorInvoiceDto>> GetMyInvoicesAsync(Guid vendorId)
        {
            var invoices = await dbContext.VendorInvoices
                .Where(vi => vi.VendorId == vendorId)
                .Include(vi => vi.Vendor)
                .Include(vi => vi.Customer)
                .Include(vi => vi.Items)
                .Include(vi => vi.VendorAssignment)
                    .ThenInclude(va => va.PartRequest)
                .Include(vi => vi.VendorAssignment)
                    .ThenInclude(va => va.Appointment)
                .OrderByDescending(vi => vi.IssuedAt)
                .ToListAsync();

            return invoices.Select(vi => MapInvoiceToDto(vi, vi.VendorAssignment));
        }

        public async Task<VendorInvoiceDto?> GetMyInvoiceByIdAsync(Guid invoiceId, Guid vendorId)
        {
            var vi = await dbContext.VendorInvoices
                .Where(x => x.VendorInvoiceId == invoiceId && x.VendorId == vendorId)
                .Include(x => x.Vendor)
                .Include(x => x.Customer)
                .Include(x => x.Items)
                .Include(x => x.VendorAssignment)
                    .ThenInclude(va => va.PartRequest)
                .Include(x => x.VendorAssignment)
                    .ThenInclude(va => va.Appointment)
                .FirstOrDefaultAsync();

            return vi == null ? null : MapInvoiceToDto(vi, vi.VendorAssignment);
        }



        private static VendorAssignmentDto MapAssignmentToDto(VendorAssignment va)
        {
            var customer = va.PartRequest?.Customer ?? va.Appointment?.Customer;
            return new VendorAssignmentDto
            {
                VendorAssignmentId = va.VendorAssignmentId,
                VendorId = va.VendorId,
                VendorName = va.Vendor?.Name,
                PartRequestId = va.PartRequestId,
                PartRequestPartName = va.PartRequest?.PartName,
                AppointmentId = va.AppointmentId,
                AppointmentServiceType = va.Appointment?.ServiceType,
                AssignedByUserId = va.AssignedByUserId,
                AssignedByName = va.AssignedBy == null
                    ? null
                    : $"{va.AssignedBy.FirstName} {va.AssignedBy.LastName}".Trim(),
                AssignedAt = va.AssignedAt,
                Status = va.Status,
                Notes = va.Notes,
                RespondedAt = va.RespondedAt,
                VendorNotes = va.VendorNotes,
                VendorInvoiceId = va.VendorInvoice?.VendorInvoiceId,
                InvoiceNumber = va.VendorInvoice?.InvoiceNumber,
                CustomerId = customer?.Id,
                CustomerName = customer == null
                    ? null
                    : $"{customer.FirstName} {customer.LastName}".Trim() is { Length: > 0 } n ? n : customer.UserName,
            };
        }

        private static VendorInvoiceDto MapInvoiceToDto(VendorInvoice vi, VendorAssignment? va)
        {
            var customerName = vi.Customer == null
                ? null
                : $"{vi.Customer.FirstName} {vi.Customer.LastName}".Trim() is { Length: > 0 } n ? n : vi.Customer.UserName;

            return new VendorInvoiceDto
            {
                VendorInvoiceId = vi.VendorInvoiceId,
                InvoiceNumber = vi.InvoiceNumber,
                VendorId = vi.VendorId,
                VendorName = vi.Vendor?.Name,
                CustomerId = vi.CustomerId,
                CustomerName = customerName,
                CustomerEmail = vi.Customer?.Email,
                VendorAssignmentId = vi.VendorAssignmentId,
                PartRequestId = va?.PartRequestId,
                PartRequestPartName = va?.PartRequest?.PartName,
                AppointmentId = va?.AppointmentId,
                AppointmentServiceType = va?.Appointment?.ServiceType,
                Status = vi.Status,
                TotalAmount = vi.TotalAmount,
                Notes = vi.Notes,
                IssuedAt = vi.IssuedAt,
                Items = vi.Items.Select(item => new VendorInvoiceItemDto
                {
                    VendorInvoiceItemId = item.VendorInvoiceItemId,
                    Description = item.Description,
                    ItemType = item.ItemType,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice
                }).ToList()
            };
        }
    }
}


