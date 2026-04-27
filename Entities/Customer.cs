using Microsoft.AspNetCore.Identity;

namespace VehicleManagementSystem.Entities
{
    public class Customer:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        
    }
}
