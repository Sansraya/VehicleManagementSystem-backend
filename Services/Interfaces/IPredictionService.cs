using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Services.Interfaces;


    public interface IPredictionService
    {
        Task<(bool Success, string Message, VehiclePredictionResponseDto? Result)>
            PredictPartFailuresAsync(VehiclePredictionRequestDto dto);
    }
