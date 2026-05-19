using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class CustomerReportService : ICustomerReportService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;

        public CustomerReportService(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<CustomerReportSummaryDto> GetSummaryAsync(int regularThreshold, decimal highSpenderThreshold)
        {
            var customers = await userManager.GetUsersInRoleAsync("Customer");
            var customerIds = customers.Select(c => c.Id).ToHashSet();

            var sales = await dbContext.Sales
                .Where(s => customerIds.Contains(s.CustomerId))
                .ToListAsync();

            var grouped = sales
                .GroupBy(s => s.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(s => s.FinalAmount)
                })
                .ToList();

            var pending = sales
                .Where(s => s.Status == "Pending" ||
                            (s.PaymentMethod != null && s.PaymentMethod.Equals("Credit", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return new CustomerReportSummaryDto
            {
                TotalCustomers = customers.Count,
                RegularCustomers = grouped.Count(g => g.Count >= regularThreshold),
                HighSpenders = grouped.Count(g => g.Total >= highSpenderThreshold),
                PendingCreditsCount = pending.Count,
                PendingCreditsAmount = pending.Sum(s => s.FinalAmount)
            };
        }

        public async Task<IEnumerable<RegularCustomerDto>> GetRegularCustomersAsync(int minVisits)
        {
            var customers = await userManager.GetUsersInRoleAsync("Customer");
            var customerIds = customers.Select(c => c.Id).ToHashSet();

            var sales = await dbContext.Sales
                .Where(s => customerIds.Contains(s.CustomerId))
                .ToListAsync();

            var grouped = sales
                .GroupBy(s => s.CustomerId)
                .Where(g => g.Count() >= minVisits)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(s => s.FinalAmount),
                    Last = g.Max(s => s.SaleDate)
                })
                .ToList();

            var customerMap = customers.ToDictionary(c => c.Id, c => c);

            return grouped
                .Select(g =>
                {
                    customerMap.TryGetValue(g.CustomerId, out var u);
                    return new RegularCustomerDto
                    {
                        CustomerId = g.CustomerId,
                        FullName = u == null ? "Unknown" : $"{u.FirstName} {u.LastName}".Trim() ?? (u.UserName ?? "Unknown"),
                        Email = u?.Email ?? "",
                        PhoneNumber = u?.PhoneNumber,
                        VisitCount = g.Count,
                        TotalSpent = g.Total,
                        LastVisit = g.Last
                    };
                })
                .OrderByDescending(r => r.VisitCount)
                .ThenByDescending(r => r.TotalSpent)
                .ToList();
        }

        public async Task<IEnumerable<HighSpenderDto>> GetHighSpendersAsync(decimal minSpent)
        {
            var customers = await userManager.GetUsersInRoleAsync("Customer");
            var customerIds = customers.Select(c => c.Id).ToHashSet();

            var sales = await dbContext.Sales
                .Where(s => customerIds.Contains(s.CustomerId))
                .ToListAsync();

            var grouped = sales
                .GroupBy(s => s.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Total = g.Sum(s => s.FinalAmount),
                    Count = g.Count(),
                    Last = g.Max(s => s.SaleDate)
                })
                .Where(g => g.Total >= minSpent)
                .ToList();

            var customerMap = customers.ToDictionary(c => c.Id, c => c);

            return grouped
                .Select(g =>
                {
                    customerMap.TryGetValue(g.CustomerId, out var u);
                    var displayName = u == null
                        ? "Unknown"
                        : (string.IsNullOrWhiteSpace($"{u.FirstName} {u.LastName}".Trim())
                            ? (u.UserName ?? "Unknown")
                            : $"{u.FirstName} {u.LastName}".Trim());

                    return new HighSpenderDto
                    {
                        CustomerId = g.CustomerId,
                        FullName = displayName,
                        Email = u?.Email ?? "",
                        PhoneNumber = u?.PhoneNumber,
                        TotalSpent = g.Total,
                        PurchaseCount = g.Count,
                        AverageSpend = g.Count > 0 ? g.Total / g.Count : 0,
                        LastVisit = g.Last
                    };
                })
                .OrderByDescending(h => h.TotalSpent)
                .ToList();
        }

        public async Task<IEnumerable<PendingCreditDto>> GetPendingCreditsAsync()
        {
            var sales = await dbContext.Sales
                .Include(s => s.Customer)
                .Where(s => s.Status == "Pending" ||
                            (s.PaymentMethod != null &&
                             (s.PaymentMethod.ToLower() == "credit" || s.PaymentMethod.ToLower() == "due")))
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            var invoices = await dbContext.Invoices
                .Where(i => sales.Select(s => s.SaleId).Contains(i.SaleId))
                .ToListAsync();
            var invoiceMap = invoices.ToDictionary(i => i.SaleId, i => i);

            var now = DateTime.UtcNow;

            return sales.Select(s =>
            {
                invoiceMap.TryGetValue(s.SaleId, out var inv);
                var displayName = s.Customer == null
                    ? "Unknown"
                    : (string.IsNullOrWhiteSpace($"{s.Customer.FirstName} {s.Customer.LastName}".Trim())
                        ? (s.Customer.UserName ?? "Unknown")
                        : $"{s.Customer.FirstName} {s.Customer.LastName}".Trim());

                return new PendingCreditDto
                {
                    SaleId = s.SaleId,
                    InvoiceNumber = inv?.InvoiceNumber ?? "N/A",
                    CustomerId = s.CustomerId,
                    CustomerName = displayName,
                    CustomerPhone = s.Customer?.PhoneNumber,
                    SaleDate = s.SaleDate,
                    Amount = s.FinalAmount,
                    PaymentMethod = s.PaymentMethod,
                    Status = s.Status,
                    DaysOutstanding = (int)Math.Max(0, (now - s.SaleDate).TotalDays)
                };
            }).ToList();
        }
    }
}