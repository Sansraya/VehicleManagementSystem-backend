using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Dtos.Models
{
    public class RegisterModel
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Firstname must be between 3 and 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Secondname must be between 3 and 50 characters")]
        public string? LastName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 50 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"^(98|97)\d{8}$", ErrorMessage = "Enter a valid Nepali phone number")]
        public string? Phone { get; set; }

    }
}
