using Microsoft.AspNetCore.Identity;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Data
{
    public class AdminSeeder
    {
        public static async Task SeedAdmin(UserManager<User> userManager)
        {
            const string adminUsername = "admin";
            const string adminEmail = "admin@yahnepal.com";
            const string adminPassword = "Admin@123";

            var existing = await userManager.FindByNameAsync(adminUsername);
            if (existing != null) return;

            var admin = new User
            {
                UserName = adminUsername,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}