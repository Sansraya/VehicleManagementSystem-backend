namespace VehicleManagementSystem.Dtos
{
    public class ReviewOptionResponseDto
    {
        public Guid Id { get; set; }

        public Guid? VendorId { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}