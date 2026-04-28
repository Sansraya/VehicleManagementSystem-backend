using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Entities
{
    public class Appointment
    {
        [Key]
        public Guid AppointmentId { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        public User? Customer { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}