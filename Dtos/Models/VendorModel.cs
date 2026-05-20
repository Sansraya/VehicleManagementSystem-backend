using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Dtos.Models
{
    public class VendorModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

      
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? LastName { get; set; }

        [RegularExpression(@"^(98|97)\d{8}$", ErrorMessage = "Enter a valid Nepali phone number")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? ContactPerson { get; set; }
    }
}