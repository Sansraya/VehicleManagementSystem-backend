using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IStaffService
    {
        Task<(bool Success, string Message)> RegisterStaffAsync(StaffRegisterModel model);
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
        Task<StaffDto?> GetStaffByIdAsync(string id);
        Task<(bool Success, string Message)> UpdateStaffAsync(string id, UserDto model);
        Task<(bool Success, string Message)> DeleteStaffAsync(string id);
    }
}