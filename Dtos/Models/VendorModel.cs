using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Dtos.Models
{
    public class VendorModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string ContactPerson { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Phone { get; set; }

        [Required]
        public required string Address { get; set; }
    }
}