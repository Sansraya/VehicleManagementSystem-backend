using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext dbContext;

        public InvoiceService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, string? InvoiceNumber, decimal? TotalAmount)> CreateInvoiceAsync(PurchaseInvoiceModel model)
        {
            var vendor = await dbContext.Vendors.FindAsync(model.VendorId);
            if (vendor == null) return (false, "Vendor not found", null, null);

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
                if (part == null) return (false, $"Part {item.PartsId} not found", null, null);

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
            return (true, "Invoice created", invoice.InvoiceNumber, invoice.TotalAmount);
        }

        public async Task<IEnumerable<PurchaseInvoiceDto>> GetAllInvoicesAsync()
        {
            return await dbContext.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .Select(i => new PurchaseInvoiceDto
                {
                    PurchaseInvoiceId = i.PurchaseInvoiceId,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    Status = i.Status,
                    TotalAmount = i.TotalAmount,
                    Notes = i.Notes,
                    CreatedAt = i.CreatedAt,
                    Vendor = new VendorSummaryDto
                    {
                        VendorId = i.Vendor.VendorId,
                        Name = i.Vendor.Name
                    },
                    Items = i.Items.Select(item => new PurchaseInvoiceItemDto
                    {
                        PartsId = item.PartsId,
                        PartName = item.Part.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.Quantity * item.UnitPrice
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<PurchaseInvoiceDto?> GetInvoiceByIdAsync(Guid id)
        {
            var invoice = await dbContext.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .FirstOrDefaultAsync(i => i.PurchaseInvoiceId == id);

            if (invoice == null) return null;

            return new PurchaseInvoiceDto
            {
                PurchaseInvoiceId = invoice.PurchaseInvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                Status = invoice.Status,
                TotalAmount = invoice.TotalAmount,
                Notes = invoice.Notes,
                CreatedAt = invoice.CreatedAt,
                Vendor = new VendorSummaryDto
                {
                    VendorId = invoice.Vendor.VendorId,
                    Name = invoice.Vendor.Name
                },
                Items = invoice.Items.Select(item => new PurchaseInvoiceItemDto
                {
                    PartsId = item.PartsId,
                    PartName = item.Part.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message)> UpdateInvoiceStatusAsync(Guid id, string status)
        {
            var allowed = new[] { "Pending", "Received", "Cancelled" };
            if (!allowed.Contains(status)) return (false, "Invalid status. Use: Pending, Received, Cancelled");

            var invoice = await dbContext.PurchaseInvoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.PurchaseInvoiceId == id);

            if (invoice == null) return (false, "Invoice not found");

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
            return (true, $"Invoice marked as {status}");
        }
    }
}