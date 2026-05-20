using System.ComponentModel.DataAnnotations;

namespace VehicleManagementSystem.Dtos
{
    public class UpdateCustomerModel
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [RegularExpression(@"^(98|97)\d{8}$", ErrorMessage = "Enter a valid Nepali phone number")]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }
    }
}
