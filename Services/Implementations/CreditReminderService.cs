using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations;

public class CreditReminderService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<CreditReminderService> logger;
    private readonly TimeSpan checkInterval = TimeSpan.FromHours(24);

    public CreditReminderService(IServiceScopeFactory scopeFactory, ILogger<CreditReminderService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Credit Reminder Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendOverdueCreditRemindersAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while sending credit reminders.");
            }

            await Task.Delay(checkInterval, stoppingToken);
        }
    }

    private async Task SendOverdueCreditRemindersAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var cutoffDate = DateTime.UtcNow.AddDays(-30);

        var overdueSales = await dbContext.Sales
            .Include(s => s.Customer)
            .Include(s => s.Invoice)
            .Include(s => s.SaleItems)
            .Where(s =>
                (s.Status == "Pending" ||
                 (s.PaymentMethod != null && s.PaymentMethod.ToLower() == "credit")) &&
                s.SaleDate <= cutoffDate)
            .OrderBy(s => s.SaleDate)
            .ToListAsync();

        if (!overdueSales.Any())
        {
            logger.LogInformation("No overdue credit balances found.");
            return;
        }

        logger.LogInformation("Found {Count} overdue credit sale(s). Sending reminders...", overdueSales.Count);

        foreach (var sale in overdueSales)
        {
            var customerEmail = sale.Customer?.Email;
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                logger.LogWarning("Skipping sale {SaleId}: customer has no email.", sale.SaleId);
                continue;
            }

            var customerName = $"{sale.Customer?.FirstName} {sale.Customer?.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(customerName))
                customerName = sale.Customer?.UserName ?? "Valued Customer";

            var invoiceNumber = sale.Invoice?.InvoiceNumber ?? $"INV-{sale.SaleId.ToString()[..8].ToUpper()}";
            var daysOverdue = (int)(DateTime.UtcNow - sale.SaleDate).TotalDays;

            try
            {
                await emailService.SendCreditReminderEmailAsync(
                    customerEmail,
                    customerName,
                    invoiceNumber,
                    sale.SaleDate,
                    sale.FinalAmount,
                    daysOverdue);

                logger.LogInformation(
                    "Reminder sent to {Email} for invoice {Invoice} ({Days} days overdue).",
                    customerEmail, invoiceNumber, daysOverdue);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send reminder to {Email} for sale {SaleId}.", customerEmail,
                    sale.SaleId);
            }
        }
    }
}