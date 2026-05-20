using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interfaces;

public interface IAdminRecommendationService
{
    Task<(bool Success, string Message, AdminRecommendationResponseDto? Result)>
        GetRecommendationsAsync();
}