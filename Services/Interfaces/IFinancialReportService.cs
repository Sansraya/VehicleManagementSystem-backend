using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IFinancialReportService
    {
        Task<FinancialReportDto> GetDailyReportAsync(DateTime date);
        Task<FinancialReportDto> GetMonthlyReportAsync(int year, int month);
        Task<FinancialReportDto> GetYearlyReportAsync(int year);
        Task<FinancialReportDto> GetRangeReportAsync(DateTime start, DateTime end);
    }
}
