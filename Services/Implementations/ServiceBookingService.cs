using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations;

public class ServiceBookingService : IServiceBookingService
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<User> userManager;

    private static readonly string[] ValidStatuses =
        ["Pending", "Confirmed", "InProgress", "Completed", "Cancelled"];

    public ServiceBookingService(ApplicationDbContext db, UserManager<User> userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }
    

    public async Task<(bool Success, string Message, BookingResponseDto? Booking)> CreateByCustomerAsync(
        string customerId, CreateBookingDto dto)
    {
        var customer = await userManager.FindByIdAsync(customerId);
        if (customer == null)
            return (false, "Customer not found.", null);

        var (valid, validationError) = await ValidateCreateDto(dto, vehicleOwnerId: customerId);
        if (!valid) return (false, validationError!, null);

        var booking = BuildBooking(dto, customerId, createdById: customerId, status: "Pending");
        await db.ServiceBookings.AddAsync(booking);
        await db.SaveChangesAsync();

        return (true, "Booking created successfully.", await LoadDto(booking.BookingId));
    }

    public async Task<(bool Success, string Message, BookingResponseDto? Booking)> CreateByStaffAsync(
        string staffId, CreateBookingDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerId))
            return (false, "CustomerId is required when staff creates a booking.", null);

        var customer = await userManager.FindByIdAsync(dto.CustomerId);
        if (customer == null)
            return (false, "Customer not found.", null);
        
        if (!string.IsNullOrWhiteSpace(dto.AssignedStaffId))
        {
            var assignee = await userManager.FindByIdAsync(dto.AssignedStaffId);
            if (assignee == null)
                return (false, "Assigned staff member not found.", null);

            var isStaff = await userManager.IsInRoleAsync(assignee, "Staff")
                       || await userManager.IsInRoleAsync(assignee, "Admin");
            if (!isStaff)
                return (false, "AssignedStaffId must belong to a Staff or Admin user.", null);
        }

        var (valid, validationError) = await ValidateCreateDto(dto, vehicleOwnerId: dto.CustomerId);
        if (!valid) return (false, validationError!, null);

        var booking = BuildBooking(dto, dto.CustomerId, createdById: staffId, status: "Confirmed");
        booking.AssignedStaffId = string.IsNullOrWhiteSpace(dto.AssignedStaffId) ? null : dto.AssignedStaffId;
        booking.EstimatedCost = dto.EstimatedCost;

        await db.ServiceBookings.AddAsync(booking);
        await db.SaveChangesAsync();

        return (true, "Booking created successfully.", await LoadDto(booking.BookingId));
    }


    public async Task<BookingResponseDto?> GetByIdAsync(Guid bookingId)
        => await LoadDto(bookingId);

    public async Task<IEnumerable<BookingResponseDto>> GetAllAsync(
        string? status, string? serviceType, DateTime? from, DateTime? to)
    {
        var query = db.ServiceBookings
            .Include(b => b.Customer)
            .Include(b => b.AssignedStaff)
            .Include(b => b.CreatedBy)
            .Include(b => b.Vehicle)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(b => b.Status == status);

        if (!string.IsNullOrEmpty(serviceType))
            query = query.Where(b => b.ServiceType.ToLower().Contains(serviceType.ToLower()));

        if (from.HasValue)
            query = query.Where(b => b.ScheduledAt >= from.Value.ToUniversalTime());

        if (to.HasValue)
            query = query.Where(b => b.ScheduledAt <= to.Value.ToUniversalTime());

        var bookings = await query
            .OrderByDescending(b => b.ScheduledAt)
            .ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetByCustomerAsync(string customerId)
    {
        var bookings = await db.ServiceBookings
            .Include(b => b.Customer)
            .Include(b => b.AssignedStaff)
            .Include(b => b.CreatedBy)
            .Include(b => b.Vehicle)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.ScheduledAt)
            .ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetByStaffAsync(string staffId)
    {
        var bookings = await db.ServiceBookings
            .Include(b => b.Customer)
            .Include(b => b.AssignedStaff)
            .Include(b => b.CreatedBy)
            .Include(b => b.Vehicle)
            .Where(b => b.AssignedStaffId == staffId)
            .OrderByDescending(b => b.ScheduledAt)
            .ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<(bool Success, string Message, BookingResponseDto? Booking)> UpdateByStaffAsync(
        Guid bookingId, UpdateBookingDto dto)
    {
        var booking = await db.ServiceBookings.FindAsync(bookingId);
        if (booking == null)
            return (false, "Booking not found.", null);
        
        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            if (!ValidStatuses.Contains(dto.Status))
                return (false, $"Invalid status. Valid values: {string.Join(", ", ValidStatuses)}", null);

            var (canTransition, transitionError) = ValidateTransition(booking.Status, dto.Status);
            if (!canTransition) return (false, transitionError!, null);

            booking.Status = dto.Status;

            if (dto.Status == "Completed")
                booking.CompletedAt = DateTime.UtcNow;

            if (dto.Status == "Cancelled" && !string.IsNullOrWhiteSpace(dto.CancellationReason))
                booking.CancellationReason = dto.CancellationReason;
        }
        
        if (dto.AssignedStaffId is not null)
        {
            if (dto.AssignedStaffId == "")
            {
                booking.AssignedStaffId = null;
            }
            else
            {
                var assignee = await userManager.FindByIdAsync(dto.AssignedStaffId);
                if (assignee == null)
                    return (false, "Assigned staff member not found.", null);

                var isStaff = await userManager.IsInRoleAsync(assignee, "Staff")
                           || await userManager.IsInRoleAsync(assignee, "Admin");
                if (!isStaff)
                    return (false, "AssignedStaffId must belong to a Staff or Admin user.", null);

                booking.AssignedStaffId = dto.AssignedStaffId;
            }
        }

        if (dto.StaffNotes is not null)    booking.StaffNotes    = dto.StaffNotes;
        if (dto.EstimatedCost.HasValue)    booking.EstimatedCost = dto.EstimatedCost;
        if (dto.FinalCost.HasValue)        booking.FinalCost     = dto.FinalCost;
        if (dto.ScheduledAt.HasValue)      booking.ScheduledAt   = dto.ScheduledAt.Value.ToUniversalTime();
        if (!string.IsNullOrWhiteSpace(dto.ServiceType))   booking.ServiceType  = dto.ServiceType;
        if (dto.Description is not null)   booking.Description   = dto.Description;

        booking.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return (true, "Booking updated.", await LoadDto(bookingId));
    }
    

    public async Task<(bool Success, string Message, BookingResponseDto? Booking)> UpdateByCustomerAsync(
        Guid bookingId, string customerId, CustomerUpdateBookingDto dto)
    {
        var booking = await db.ServiceBookings.FindAsync(bookingId);
        if (booking == null)
            return (false, "Booking not found.", null);

        if (booking.CustomerId != customerId)
            return (false, "You are not authorised to modify this booking.", null);
        
        if (booking.Status != "Pending" && booking.Status != "Confirmed")
            return (false, $"Booking cannot be modified once it is {booking.Status}.", null);
        
        if (!string.IsNullOrWhiteSpace(dto.CancellationReason))
        {
            booking.Status = "Cancelled";
            booking.CancellationReason = dto.CancellationReason;
        }
        else
        {
            if (dto.ScheduledAt.HasValue)
            {
                if (dto.ScheduledAt.Value <= DateTime.UtcNow)
                    return (false, "Scheduled time must be in the future.", null);

                booking.ScheduledAt = dto.ScheduledAt.Value.ToUniversalTime();
                if (booking.Status == "Confirmed")
                    booking.Status = "Pending";
            }

            if (dto.Description is not null)
                booking.Description = dto.Description;
        }

        booking.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return (true, "Booking updated.", await LoadDto(bookingId));
    }
    

    private async Task<(bool Valid, string? Error)> ValidateCreateDto(
        CreateBookingDto dto, string vehicleOwnerId)
    {
        if (string.IsNullOrWhiteSpace(dto.ServiceType))
            return (false, "ServiceType is required.");

        if (dto.ScheduledAt <= DateTime.UtcNow)
            return (false, "ScheduledAt must be in the future.");

        if (dto.VehicleId.HasValue)
        {
            var vehicle = await db.Vehicles.FindAsync(dto.VehicleId.Value);
            if (vehicle == null)
                return (false, "Vehicle not found.");

            if (vehicle.OwnerId != vehicleOwnerId)
                return (false, "Vehicle does not belong to this customer.");
        }

        return (true, null);
    }

    private static ServiceBooking BuildBooking(
        CreateBookingDto dto, string customerId, string createdById, string status)
        => new()
        {
            BookingId    = Guid.NewGuid(),
            CustomerId   = customerId,
            CreatedById  = createdById,
            VehicleId    = dto.VehicleId,
            ServiceType  = dto.ServiceType,
            Description  = dto.Description,
            ScheduledAt  = dto.ScheduledAt.ToUniversalTime(),
            Status       = status,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };
    
    private static (bool, string?) ValidateTransition(string current, string next)
    {
        var allowed = current switch
        {
            "Pending"    => new[] { "Confirmed", "Cancelled" },
            "Confirmed"  => new[] { "InProgress", "Cancelled" },
            "InProgress" => new[] { "Completed", "Cancelled" },
            _            => Array.Empty<string>()
        };

        if (!allowed.Contains(next))
            return (false, $"Cannot transition from '{current}' to '{next}'.");

        return (true, null);
    }

    private async Task<BookingResponseDto?> LoadDto(Guid bookingId)
    {
        var booking = await db.ServiceBookings
            .Include(b => b.Customer)
            .Include(b => b.AssignedStaff)
            .Include(b => b.CreatedBy)
            .Include(b => b.Vehicle)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        return booking is null ? null : MapToDto(booking);
    }

    private static BookingResponseDto MapToDto(ServiceBooking b) => new()
    {
        BookingId          = b.BookingId,
        Status             = b.Status,
        ServiceType        = b.ServiceType,
        Description        = b.Description,
        ScheduledAt        = b.ScheduledAt,
        CompletedAt        = b.CompletedAt,
        CancellationReason = b.CancellationReason,
        StaffNotes         = b.StaffNotes,
        EstimatedCost      = b.EstimatedCost,
        FinalCost          = b.FinalCost,
        CreatedAt          = b.CreatedAt,
        UpdatedAt          = b.UpdatedAt,
        Customer = b.Customer is null ? null : new BookingUserSummary
        {
            Id       = b.Customer.Id,
            FullName = $"{b.Customer.FirstName} {b.Customer.LastName}".Trim(),
            Email    = b.Customer.Email,
            Phone    = b.Customer.PhoneNumber
        },
        AssignedStaff = b.AssignedStaff is null ? null : new BookingUserSummary
        {
            Id       = b.AssignedStaff.Id,
            FullName = $"{b.AssignedStaff.FirstName} {b.AssignedStaff.LastName}".Trim(),
            Email    = b.AssignedStaff.Email,
            Phone    = b.AssignedStaff.PhoneNumber
        },
        CreatedBy = b.CreatedBy is null ? null : new BookingUserSummary
        {
            Id       = b.CreatedBy.Id,
            FullName = $"{b.CreatedBy.FirstName} {b.CreatedBy.LastName}".Trim(),
            Email    = b.CreatedBy.Email
        },
        Vehicle = b.Vehicle is null ? null : new BookingVehicleSummary
        {
            VehicleId     = b.Vehicle.VehicleId,
            Brand         = b.Vehicle.Brand,
            Model         = b.Vehicle.Model,
            VehicleNumber = b.Vehicle.VehicleNumber,
            FuelType      = b.Vehicle.FuelType
        }
    };
}