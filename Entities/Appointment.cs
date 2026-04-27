using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Entities
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public Customer? Customer { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}