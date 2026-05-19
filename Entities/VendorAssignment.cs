using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities
{
    public class VendorAssignment
    {
        [Key]
        public Guid VendorAssignmentId { get; set; } = Guid.NewGuid();

        
        [ForeignKey("PartRequest")]
        public Guid? PartRequestId { get; set; }
        public PartRequest? PartRequest { get; set; }

        [ForeignKey("Appointment")]
        public Guid? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }
        public required Vendor Vendor { get; set; }

        
        [ForeignKey("AssignedBy")]
        public string AssignedByUserId { get; set; } = string.Empty;
        public User? AssignedBy { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

       
        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        // Populated when vendor responds
        public DateTime? RespondedAt { get; set; }
        public string? VendorNotes { get; set; }

        // Navigation to the invoice generated on acceptance (nullable)
        public VendorInvoice? VendorInvoice { get; set; }
    }
}
