using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities;

public class ServiceBooking
{
    [Key]
    public Guid BookingId { get; set; } = Guid.NewGuid();
    
    [ForeignKey("Customer")]
    public string CustomerId { get; set; } = string.Empty;
    public User? Customer { get; set; }
    
    [ForeignKey("AssignedStaff")]
    public string? AssignedStaffId { get; set; }
    public User? AssignedStaff { get; set; }
    
    [ForeignKey("CreatedBy")]
    public string CreatedById { get; set; } = string.Empty;
    public User? CreatedBy { get; set; }
    
    [ForeignKey("Vehicle")]
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Required]
    public string ServiceType { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    public DateTime? CompletedAt { get; set; }


    public string Status { get; set; } = "Pending";

    public string? CancellationReason { get; set; }

    public string? StaffNotes { get; set; }   

    public decimal? EstimatedCost { get; set; }

    public decimal? FinalCost { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}