namespace VehicleManagementSystem.Models
{
    public class AppointmentDto
    {
        public string CustomerId { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}