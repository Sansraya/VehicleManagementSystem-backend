using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Response;

namespace VehicleManagementSystem.Services.Interfaces;

public interface ISaleService
{
    Task<(bool Success, string Message, SaleResponseDto? Invoice)> CreateSale(
        CreateSaleDto dto, string staffId);

    Task<(bool Success, string Message, SaleResponseDto? Invoice)> GetInvoiceById(
        Guid saleId);

    Task<(bool Success, string Message, List<SaleResponseDto> Invoices)> GetAllInvoices();

    Task<(bool Success, string Message, List<SaleResponseDto> Invoices)> GetInvoicesByCustomer(
        string customerId);
}