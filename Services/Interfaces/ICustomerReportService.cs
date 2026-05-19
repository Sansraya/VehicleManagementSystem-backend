using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface ICustomerReportService
    {
        Task<CustomerReportSummaryDto> GetSummaryAsync(int regularThreshold, decimal highSpenderThreshold);
        Task<IEnumerable<RegularCustomerDto>> GetRegularCustomersAsync(int minVisits);
        Task<IEnumerable<HighSpenderDto>> GetHighSpendersAsync(decimal minSpent);
        Task<IEnumerable<PendingCreditDto>> GetPendingCreditsAsync();
    }
}
