namespace VehicleManagementSystem.Models
{
    public class PartRequestDto
    {
        public string CustomerId { get; set; } = string.Empty;

        public string PartName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}