using Microsoft.AspNetCore.Identity;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly UserManager<User> userManager;

        public StaffService(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<(bool Success, string Message)> RegisterStaffAsync(StaffRegisterModel model)
        {
            if (await userManager.FindByNameAsync(model.Username) != null)
                return (false, "Username already exists");

            if (await userManager.FindByEmailAsync(model.Email) != null)
                return (false, "Email already exists");

            var staff = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(staff, model.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(staff, "Staff");
            return (true, "Staff registered successfully");
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            var staffUsers = await userManager.GetUsersInRoleAsync("Staff");
            return staffUsers.Select(u => new StaffDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber
            });
        }

        public async Task<StaffDto?> GetStaffByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null || !await userManager.IsInRoleAsync(user, "Staff"))
                return null;

            return new StaffDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<(bool Success, string Message)> UpdateStaffAsync(string id, UserDto model)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null || !await userManager.IsInRoleAsync(user, "Staff"))
                return (false, "Staff not found");

            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Address = model.Address ?? user.Address;
            user.DateOfBirth = model.DateOfBirth ?? user.DateOfBirth;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, "Staff updated successfully");
        }

        public async Task<(bool Success, string Message)> DeleteStaffAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null || !await userManager.IsInRoleAsync(user, "Staff"))
                return (false, "Staff not found");

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, "Staff deleted successfully");
        }
    }
}