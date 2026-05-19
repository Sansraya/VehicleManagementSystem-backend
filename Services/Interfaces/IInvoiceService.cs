using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<(bool Success, string Message, string? InvoiceNumber, decimal? TotalAmount)> CreateInvoiceAsync(PurchaseInvoiceModel model);
        Task<IEnumerable<PurchaseInvoiceDto>> GetAllInvoicesAsync();
        Task<PurchaseInvoiceDto?> GetInvoiceByIdAsync(Guid id);
        Task<(bool Success, string Message)> UpdateInvoiceStatusAsync(Guid id, string status);
    }
}