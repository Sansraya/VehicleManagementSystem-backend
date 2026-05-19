using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class FinancialReportService : IFinancialReportService
    {
        private readonly ApplicationDbContext dbContext;

        public FinancialReportService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<FinancialReportDto> GetDailyReportAsync(DateTime date)
        {
            var start = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var end = start.AddDays(1).AddTicks(-1);
            var report = await BuildReportAsync(start, end, "Daily");

            // breakdown: hourly buckets for daily view
            var sales = await dbContext.Sales
                .Where(s => s.SaleDate >= start && s.SaleDate <= end)
                .ToListAsync();

            report.Breakdown = sales
                .GroupBy(s => s.SaleDate.Hour)
                .OrderBy(g => g.Key)
                .Select(g => new RevenueBreakdownDto
                {
                    Label = $"{g.Key:D2}:00",
                    Revenue = g.Sum(s => s.TotalAmount),
                    Discount = g.Sum(s => s.DiscountAmount),
                    NetRevenue = g.Sum(s => s.FinalAmount),
                    SaleCount = g.Count()
                })
                .ToList();

            return report;
        }

        public async Task<FinancialReportDto> GetMonthlyReportAsync(int year, int month)
        {
            var start = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
            var end = start.AddMonths(1).AddTicks(-1);
            var report = await BuildReportAsync(start, end, "Monthly");

            
            var sales = await dbContext.Sales
                .Where(s => s.SaleDate >= start && s.SaleDate <= end)
                .ToListAsync();

            report.Breakdown = sales
                .GroupBy(s => s.SaleDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new RevenueBreakdownDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(s => s.TotalAmount),
                    Discount = g.Sum(s => s.DiscountAmount),
                    NetRevenue = g.Sum(s => s.FinalAmount),
                    SaleCount = g.Count()
                })
                .ToList();

            return report;
        }

        public async Task<FinancialReportDto> GetYearlyReportAsync(int year)
        {
            var start = DateTime.SpecifyKind(new DateTime(year, 1, 1), DateTimeKind.Utc);
            var end = start.AddYears(1).AddTicks(-1);
            var report = await BuildReportAsync(start, end, "Yearly");

            
            var sales = await dbContext.Sales
                .Where(s => s.SaleDate >= start && s.SaleDate <= end)
                .ToListAsync();

            report.Breakdown = sales
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new RevenueBreakdownDto
                {
                    Label = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(g.Key.Month) + " " + g.Key.Year,
                    Revenue = g.Sum(s => s.TotalAmount),
                    Discount = g.Sum(s => s.DiscountAmount),
                    NetRevenue = g.Sum(s => s.FinalAmount),
                    SaleCount = g.Count()
                })
                .ToList();

            return report;
        }

        public async Task<FinancialReportDto> GetRangeReportAsync(DateTime start, DateTime end)
        {
            var s = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
            var e = DateTime.SpecifyKind(end.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            return await BuildReportAsync(s, e, "Custom");
        }

        private async Task<FinancialReportDto> BuildReportAsync(DateTime start, DateTime end, string period)
        {
            var sales = await dbContext.Sales
                .Where(s => s.SaleDate >= start && s.SaleDate <= end)
                .ToListAsync();

            var purchaseInvoices = await dbContext.PurchaseInvoices
                .Where(p => p.InvoiceDate >= start && p.InvoiceDate <= end)
                .ToListAsync();

            var totalRevenue = sales.Sum(s => s.TotalAmount);
            var totalDiscount = sales.Sum(s => s.DiscountAmount);
            var netRevenue = sales.Sum(s => s.FinalAmount);
            var totalPurchase = purchaseInvoices.Sum(p => p.TotalAmount);
            var outstanding = purchaseInvoices
                .Where(p => p.Status != "Received" && p.Status != "Cancelled")
                .Sum(p => p.TotalAmount);

            var paymentMethods = sales
                .GroupBy(s => string.IsNullOrWhiteSpace(s.PaymentMethod) ? "Unknown" : s.PaymentMethod)
                .Select(g => new PaymentMethodBreakdownDto
                {
                    PaymentMethod = g.Key,
                    Total = g.Sum(s => s.FinalAmount),
                    Count = g.Count()
                })
                .OrderByDescending(p => p.Total)
                .ToList();

            return new FinancialReportDto
            {
                Period = period,
                StartDate = start,
                EndDate = end,
                TotalRevenue = totalRevenue,
                TotalDiscount = totalDiscount,
                NetRevenue = netRevenue,
                TotalPurchaseCost = totalPurchase,
                GrossProfit = netRevenue - totalPurchase,
                TotalSales = sales.Count,
                CompletedSales = sales.Count(s => s.Status == "Completed"),
                PendingSales = sales.Count(s => s.Status == "Pending"),
                TotalPurchaseInvoices = purchaseInvoices.Count,
                OutstandingPayables = outstanding,
                PaymentMethods = paymentMethods
            };
        }
    }
}