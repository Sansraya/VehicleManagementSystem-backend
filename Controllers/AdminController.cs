using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;
using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;

        public AdminController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }


        [HttpPost("staff/register")]
        public async Task<IActionResult> RegisterStaff([FromForm] StaffRegisterModel model)
        {
            if (await userManager.FindByNameAsync(model.Username) != null)
                return BadRequest(new { message = "Username already exists" });

            if (await userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Email already exists" });

            var staff = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(staff, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(staff, "Staff");
            return Ok(new { message = "Staff registered successfully" });
        }

        [HttpGet("staff")]
        public async Task<IActionResult> GetAllStaff()
        {
            var staffUsers = await userManager.GetUsersInRoleAsync("Staff");
            var result = staffUsers.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber
            });
            return Ok(result);
        }

        [HttpGet("staff/{id}")]
        public async Task<IActionResult> GetStaff(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null || !await userManager.IsInRoleAsync(user, "Staff"))
                return NotFound(new { message = "Staff not found" });

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber
            });
        }

        [HttpPut("staff/{id}")]
        public async Task<IActionResult> UpdateStaff(string id, [FromForm] UserDto model)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null || !await userManager.IsInRoleAsync(user, "Staff"))
                return NotFound(new { message = "Staff not found" });

            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Address = model.Address ?? user.Address;
            user.DateOfBirth = model.DateOfBirth ?? user.DateOfBirth;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Staff updated successfully" });
        }

        [HttpDelete("staff/{id}")]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null || !await userManager.IsInRoleAsync(user, "Staff"))
                return NotFound(new { message = "Staff not found" });

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Staff deleted successfully" });
        }
        [HttpPost("vendors")]
        public async Task<IActionResult> CreateVendor([FromForm] VendorModel model)
        {
            var vendor = new Vendor
            {
                VendorId = Guid.NewGuid(),
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Vendors.AddAsync(vendor);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Vendor created", vendor.VendorId });
        }

        [HttpGet("vendors")]
        public async Task<IActionResult> GetVendors()
        {
            var vendors = await dbContext.Vendors.ToListAsync();
            return Ok(vendors);
        }

        [HttpGet("vendors/{id}")]
        public async Task<IActionResult> GetVendor(Guid id)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return NotFound(new { message = "Vendor not found" });
            return Ok(vendor);
        }

        [HttpPut("vendors/{id}")]
        public async Task<IActionResult> UpdateVendor(Guid id, [FromForm] VendorModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return NotFound(new { message = "Vendor not found" });

            vendor.Name = model.Name;
            vendor.ContactPerson = model.ContactPerson;
            vendor.Email = model.Email;
            vendor.Phone = model.Phone;
            vendor.Address = model.Address;

            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Vendor updated" });
        }

        [HttpDelete("vendors/{id}")]
        public async Task<IActionResult> DeleteVendor(Guid id)
        {
            var vendor = await dbContext.Vendors.FindAsync(id);
            if (vendor == null) return NotFound(new { message = "Vendor not found" });

            dbContext.Vendors.Remove(vendor);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Vendor deleted" });
        }

        [HttpPost("parts")]
        public async Task<IActionResult> CreatePart([FromForm] PartModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
            if (vendor == null) return BadRequest(new { message = "Vendor not found" });

            var part = new Parts
            {
                PartsId = Guid.NewGuid(),
                Name = model.Name,
                PartCode = model.PartCode,
                Category = model.Category,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                VendorId = model.VendorId,
                Vendor = vendor,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await dbContext.Parts.AddAsync(part);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Part created", part.PartsId });
        }

        [HttpGet("parts")]
        public async Task<IActionResult> GetParts()
        {
            var parts = await dbContext.Parts
                .Include(p => p.Vendor)
                .Select(p => new
                {
                    p.PartsId,
                    p.Name,
                    p.PartCode,
                    p.Category,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.CreatedAt,
                    p.UpdatedAt,
                    Vendor = new { p.Vendor.VendorId, p.Vendor.Name }
                })
                .ToListAsync();
            return Ok(parts);
        }

        [HttpGet("parts/{id}")]
        public async Task<IActionResult> GetPart(Guid id)
        {
            var part = await dbContext.Parts
                .Include(p => p.Vendor)
                .Where(p => p.PartsId == id)
                .Select(p => new
                {
                    p.PartsId,
                    p.Name,
                    p.PartCode,
                    p.Category,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    Vendor = new { p.Vendor.VendorId, p.Vendor.Name }
                })
                .FirstOrDefaultAsync();

            if (part == null) return NotFound(new { message = "Part not found" });
            return Ok(part);
        }

        [HttpPut("parts/{id}")]
        public async Task<IActionResult> UpdatePart(Guid id, [FromForm] PartModel model)
        {
            var part = await dbContext.Parts.FindAsync(id);
            if (part == null) return NotFound(new { message = "Part not found" });

            if (model.VendorId != part.VendorId)
            {
                var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
                if (vendor == null) return BadRequest(new { message = "Vendor not found" });
            }

            part.Name = model.Name;
            part.PartCode = model.PartCode;
            part.Category = model.Category;
            part.Description = model.Description;
            part.Price = model.Price;
            part.StockQuantity = model.StockQuantity;
            part.VendorId = model.VendorId;
            part.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Part updated" });
        }

        [HttpDelete("parts/{id}")]
        public async Task<IActionResult> DeletePart(Guid id)
        {
            var part = await dbContext.Parts.FindAsync(id);
            if (part == null) return NotFound(new { message = "Part not found" });

            dbContext.Parts.Remove(part);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Part deleted" });
        }
   

        [HttpPost("invoices")]
        public async Task<IActionResult> CreateInvoice([FromBody] PurchaseInvoiceModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
            if (vendor == null) return BadRequest(new { message = "Vendor not found" });

            var invoice = new PurchaseInvoice
            {
                PurchaseInvoiceId = Guid.NewGuid(),
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                VendorId = model.VendorId,
                Vendor = vendor,
                Notes = model.Notes,
                InvoiceDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            foreach (var item in model.Items)
            {
                var part = await dbContext.Parts.FindAsync(item.PartsId);
                if (part == null)
                    return BadRequest(new { message = $"Part {item.PartsId} not found" });

                invoice.Items.Add(new PurchaseInvoiceItem
                {
                    PurchaseInvoiceItemId = Guid.NewGuid(),
                    PurchaseInvoiceId = invoice.PurchaseInvoiceId,
                    PartsId = item.PartsId,
                    Part = part,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            invoice.TotalAmount = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);

            await dbContext.PurchaseInvoices.AddAsync(invoice);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Invoice created", invoice.InvoiceNumber, invoice.TotalAmount });
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            var invoices = await dbContext.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .Select(i => new
                {
                    i.PurchaseInvoiceId,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.Status,
                    i.TotalAmount,
                    i.Notes,
                    Vendor = new { i.Vendor.VendorId, i.Vendor.Name },
                    Items = i.Items.Select(item => new
                    {
                        item.PartsId,
                        PartName = item.Part.Name,
                        item.Quantity,
                        item.UnitPrice,
                        item.SubTotal
                    })
                })
                .ToListAsync();
            return Ok(invoices);
        }

        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var invoice = await dbContext.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .Where(i => i.PurchaseInvoiceId == id)
                .Select(i => new
                {
                    i.PurchaseInvoiceId,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.Status,
                    i.TotalAmount,
                    i.Notes,
                    Vendor = new { i.Vendor.VendorId, i.Vendor.Name },
                    Items = i.Items.Select(item => new
                    {
                        item.PartsId,
                        PartName = item.Part.Name,
                        item.Quantity,
                        item.UnitPrice,
                        item.SubTotal
                    })
                })
                .FirstOrDefaultAsync();

            if (invoice == null) return NotFound(new { message = "Invoice not found" });
            return Ok(invoice);
        }

        [HttpPatch("invoices/{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(Guid id, [FromForm] string status)
        {
            var allowed = new[] { "Pending", "Received", "Cancelled" };
            if (!allowed.Contains(status))
                return BadRequest(new { message = "Invalid status. Use: Pending, Received, Cancelled" });

            var invoice = await dbContext.PurchaseInvoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.PurchaseInvoiceId == id);

            if (invoice == null) return NotFound(new { message = "Invoice not found" });

            if (status == "Received" && invoice.Status != "Received")
            {
                foreach (var item in invoice.Items)
                {
                    var part = await dbContext.Parts.FindAsync(item.PartsId);
                    if (part != null)
                    {
                        part.StockQuantity += item.Quantity;
                        part.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            invoice.Status = status;
            await dbContext.SaveChangesAsync();
            return Ok(new { message = $"Invoice marked as {status}" });
        }
    }
}