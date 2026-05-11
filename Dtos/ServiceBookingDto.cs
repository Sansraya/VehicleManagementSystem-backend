namespace VehicleManagementSystem.Dtos;

public class CreateBookingDto
{
    public string? CustomerId { get; set; }

    public Guid? VehicleId { get; set; }

    public string ServiceType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string? AssignedStaffId { get; set; }   

    public decimal? EstimatedCost { get; set; }   
}


public class UpdateBookingDto
{
    public string? Status { get; set; }
    public string? AssignedStaffId { get; set; }
    public string? StaffNotes { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? FinalCost { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? ServiceType { get; set; }
    public string? Description { get; set; }
}

public class CustomerUpdateBookingDto
{
    public DateTime? ScheduledAt { get; set; }
    public string? Description { get; set; }
    public string? CancellationReason { get; set; }
}


public class BookingResponseDto
{
    public Guid BookingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? StaffNotes { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? FinalCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public BookingUserSummary? Customer { get; set; }
    public BookingUserSummary? AssignedStaff { get; set; }
    public BookingUserSummary? CreatedBy { get; set; }
    public BookingVehicleSummary? Vehicle { get; set; }
}

public class BookingUserSummary
{
    public string Id { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class BookingVehicleSummary
{
    public Guid VehicleId { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? VehicleNumber { get; set; }
    public string? FuelType { get; set; }
}