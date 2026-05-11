using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interfaces;

public interface IServiceBookingService
{
    Task<(bool Success, string Message, BookingResponseDto? Booking)> CreateByCustomerAsync(string customerId, CreateBookingDto dto);
    
    Task<(bool Success, string Message, BookingResponseDto? Booking)> CreateByStaffAsync(string staffId, CreateBookingDto dto);


    Task<BookingResponseDto?> GetByIdAsync(Guid bookingId);
    
    Task<IEnumerable<BookingResponseDto>> GetAllAsync(string? status, string? serviceType, DateTime? from, DateTime? to);
    
    Task<IEnumerable<BookingResponseDto>> GetByCustomerAsync(string customerId);
    
    Task<IEnumerable<BookingResponseDto>> GetByStaffAsync(string staffId);

    Task<(bool Success, string Message, BookingResponseDto? Booking)> UpdateByStaffAsync(Guid bookingId, UpdateBookingDto dto);


    Task<(bool Success, string Message, BookingResponseDto? Booking)> UpdateByCustomerAsync(Guid bookingId, string customerId, CustomerUpdateBookingDto dto);
}