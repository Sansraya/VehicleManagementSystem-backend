namespace VehicleManagementSystem.Dtos
{
    public class PartRequestResponseDto
    {
        public Guid PartRequestId { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public string PartName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; }
    }
}