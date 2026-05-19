using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities
{
    public class Appointment
    {
        [Key]
        public Guid AppointmentId { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        public User? Customer { get; set; }

        public string? StaffId { get; set; }

        public User? Staff { get; set; }

        public Guid? VendorId { get; set; }

        public Vendor? Vendor { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}