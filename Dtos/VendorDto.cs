namespace VehicleManagementSystem.Dtos
{
    public class VendorDto
    {
        public Guid VendorId { get; set; }
        public string? Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}