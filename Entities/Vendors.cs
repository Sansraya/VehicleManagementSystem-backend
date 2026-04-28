using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities
{
    public class Vendor
    {
        public Guid VendorId { get;set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
