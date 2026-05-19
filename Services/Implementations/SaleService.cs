using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Response;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations;

public class SaleService(ApplicationDbContext dbContext) : ISaleService
{
    public async Task<(bool Success, string Message, SaleResponseDto? Invoice)> CreateSale(
        CreateSaleDto createSaleDto, string staffId)
    {
        var staff = await dbContext.Users.FindAsync(staffId);
        if (staff == null)
            return (false, "Staff not found.", null);

        var customer = await dbContext.Users.FindAsync(createSaleDto.CustomerId);
        if (customer == null)
            return (false, "Customer not found.", null);

        if (!createSaleDto.Items.Any())
            return (false, "Sale must have at least one item.", null);

        if (string.IsNullOrWhiteSpace(createSaleDto.PaymentMethod))
            return (false, "Payment method is required.", null);

        var saleItems = new List<SaleItem>();
        double totalAmount = 0;

        foreach (var itemDto in createSaleDto.Items)
        {
            if (itemDto.Quantity <= 0)
                return (false, "Quantity must be greater than 0.", null);

            var part = await dbContext.Parts.FindAsync(itemDto.PartsId);
            if (part == null)
                return (false, $"Part with ID {itemDto.PartsId} not found.", null);

            if (part.StockQuantity < itemDto.Quantity)
                return (false, $"Insufficient stock for '{part.Name}'. Available: {part.StockQuantity}.", null);

            var lineTotal = (double)part.Price * itemDto.Quantity;
            totalAmount += lineTotal;

            var saleItem = new SaleItem
            {
                SaleItemId = Guid.NewGuid(),
                PartsId = part.PartsId,
                ItemType = "Part",
                ItemName = part.Name,
                Quantity = itemDto.Quantity,
                UnitPrice = (double)part.Price,
                LineTotal = lineTotal
            };

            part.StockQuantity -= itemDto.Quantity;
            part.UpdatedAt = DateTime.UtcNow;
            dbContext.Parts.Update(part);

            saleItems.Add(saleItem);
        }

        var discountAmount = (double)createSaleDto.DiscountAmount;
        var finalAmount = totalAmount - discountAmount;

        if (finalAmount < 0)
            return (false, "Discount cannot exceed total amount.", null);

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var sale = new Sale
        {
            SaleId = Guid.NewGuid(),
            CustomerId = createSaleDto.CustomerId,
            Customer = customer,
            StaffId = staffId,
            Staff = staff,
            SaleDate = DateTime.UtcNow,
            TotalAmount = (decimal)totalAmount,
            DiscountAmount = createSaleDto.DiscountAmount,
            FinalAmount = (decimal)finalAmount,
            PaymentMethod = createSaleDto.PaymentMethod,
            Status = "Completed",
            Remarks = createSaleDto.Remarks,
            SaleItems = saleItems
        };

        var invoice = new Invoice
        {
            InvoiceId = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            SaleId = sale.SaleId,
            Sale = sale,
            IssuedAt = DateTime.UtcNow,
            Status = "Issued",
            EmailSent = false
        };

        await dbContext.Sales.AddAsync(sale);
        await dbContext.Invoices.AddAsync(invoice);
        await dbContext.SaveChangesAsync();

        return (true, "Sale created successfully.", MapToDto(sale, invoice));
    }

    public async Task<(bool Success, string Message, SaleResponseDto? Invoice)> GetInvoiceById(Guid saleId)
    {
        var sale = await dbContext.Sales
            .Include(s => s.Staff)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Parts)
            .Include(s => s.Invoice)
            .FirstOrDefaultAsync(s => s.SaleId == saleId);

        if (sale == null)
            return (false, "Sale not found.", null);

        return (true, "Success.", MapToDto(sale, sale.Invoice));
    }

    public async Task<(bool Success, string Message, List<SaleResponseDto> Invoices)> GetAllInvoices()
    {
        var sales = await dbContext.Sales
            .Include(s => s.Staff)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Parts)
            .Include(s => s.Invoice)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        return (true, "Success.", sales.Select(s => MapToDto(s, s.Invoice)).ToList());
    }

    public async Task<(bool Success, string Message, List<SaleResponseDto> Invoices)> GetInvoicesByCustomer(
        string customerId)
    {
        var customer = await dbContext.Users.FindAsync(customerId);
        if (customer == null)
            return (false, "Customer not found.", new List<SaleResponseDto>());

        var sales = await dbContext.Sales
            .Include(s => s.Staff)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Parts)
            .Include(s => s.Invoice)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        return (true, "Success.", sales.Select(s => MapToDto(s, s.Invoice)).ToList());
    }

    public async Task<(bool Success, string Message)> SendInvoiceEmailAsync(Guid saleId, IEmailService emailService)
    {
        var sale = await dbContext.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .Include(s => s.Invoice)
            .FirstOrDefaultAsync(s => s.SaleId == saleId);

        if (sale == null)
            return (false, "Sale not found.");

        var customerEmail = sale.Customer?.Email;
        if (string.IsNullOrWhiteSpace(customerEmail))
            return (false, "Customer does not have an email address on file.");

        var customerName = $"{sale.Customer?.FirstName} {sale.Customer?.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(customerName))
            customerName = sale.Customer?.UserName ?? "Valued Customer";

        var invoiceNumber = sale.Invoice?.InvoiceNumber ?? $"INV-{sale.SaleId.ToString()[..8].ToUpper()}";

        var items = sale.SaleItems.Select(si =>
            (si.ItemName, si.Quantity, si.UnitPrice, si.LineTotal));

        try
        {
            await emailService.SendInvoiceEmailAsync(
                customerEmail,
                customerName,
                invoiceNumber,
                sale.SaleDate,
                sale.PaymentMethod,
                sale.TotalAmount,
                sale.DiscountAmount,
                sale.FinalAmount,
                items);

            
            if (sale.Invoice != null)
            {
                sale.Invoice.EmailSent = true;
                sale.Invoice.EmailSentAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
            }

            return (true, "Email sent successfully.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to send email: {ex.Message}");
        }
    }

    private static SaleResponseDto MapToDto(Sale sale, Invoice? invoice)
    {
        return new SaleResponseDto
        {
            SaleId = sale.SaleId,
            InvoiceNumber = invoice?.InvoiceNumber ?? "N/A",
            InvoiceStatus = invoice?.Status ?? "N/A",
            IssuedAt = invoice?.IssuedAt ?? sale.SaleDate,
            StaffName = $"{sale.Staff?.FirstName} {sale.Staff?.LastName}".Trim(),
            CustomerName = $"{sale.Customer?.FirstName} {sale.Customer?.LastName}".Trim(),
            SaleDate = sale.SaleDate,
            PaymentMethod = sale.PaymentMethod,
            Status = sale.Status,
            Remarks = sale.Remarks,
            TotalAmount = sale.TotalAmount,
            DiscountAmount = sale.DiscountAmount,
            FinalAmount = sale.FinalAmount,
            Items = sale.SaleItems.Select(si => new SaleItemResponseDto
            {
                ItemType = si.ItemType,
                ItemName = si.ItemName,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                LineTotal = si.LineTotal
            }).ToList()
        };
    }
}