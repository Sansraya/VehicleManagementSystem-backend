using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Models;

public class RegisterCustomerWithVehicleModel
{

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public required string ConfirmPassword { get; set; }


    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [RegularExpression(@"^(98|97)\d{8}$", ErrorMessage = "Enter a valid Nepali phone number")]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public VehicleInputModel? Vehicle { get; set; }
}

public class VehicleInputModel
{
    [Required]
    [StringLength(50)]
    public required string Brand { get; set; }

    [Required]
    [StringLength(50)]
    public required string Model { get; set; }

    [Required]
    [StringLength(20)]
    public required string VehicleNumber { get; set; }

    [Required]
    [StringLength(30)]
    public required string Colour { get; set; }

    
    [Required]
    public required DateOnly ManufacturingYear { get; set; }

    [Required]
    [StringLength(20)]
    public required string FuelType { get; set; }

    [Range(0, double.MaxValue)]
    public double Price { get; set; }
}