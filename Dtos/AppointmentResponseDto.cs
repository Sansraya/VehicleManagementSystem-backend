namespace VehicleManagementSystem.Dtos
{
    public class AppointmentResponseDto
    {
        public Guid AppointmentId { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public string? StaffId { get; set; }

        public Guid? VendorId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}