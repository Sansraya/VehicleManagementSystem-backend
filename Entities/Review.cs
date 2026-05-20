using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;
        public User? Customer { get; set; }

        public Guid? VendorId { get; set; }
        public Vendor? Vendor { get; set; }

        public Guid? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public Guid? PartId { get; set; }
        public Parts? Part { get; set; }
        public Guid? PartRequestId { get; set; }

        public PartRequest? PartRequest { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}