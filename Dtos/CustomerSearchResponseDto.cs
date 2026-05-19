namespace VehicleManagementSystem.Dtos
{
    public class CustomerSearchResponseDto
    {
        public string CustomerId { get; set; } = string.Empty;

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public Guid VehicleId { get; set; }

        public string VehicleNumber { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
    }
}