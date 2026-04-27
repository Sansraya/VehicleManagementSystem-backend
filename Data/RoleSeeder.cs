using Microsoft.AspNetCore.Identity;

namespace VehicleManagementSystem.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Customer", "Staff", "Admin"];

            foreach (string r in roles)
            {
                var result = await roleManager.RoleExistsAsync(r);
                if (!result)
                {
                    await roleManager.CreateAsync(new IdentityRole(r));
                }
            }

        }
    }
}
