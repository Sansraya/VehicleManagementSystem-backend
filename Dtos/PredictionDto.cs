namespace VehicleManagementSystem.Dtos;

public class VehiclePredictionRequestDto
{
    public string CustomerId { get; set; } = string.Empty;
    public int TotalMileageKm { get; set; }
    public int MonthlyDrivingKm { get; set; }
    public string RoadType { get; set; } = string.Empty;
    public int MonthsSinceLastService { get; set; }
    public string? KnownSymptoms { get; set; }
}

public class PartPredictionDto
{
    public string Part { get; set; } = string.Empty;
    public string Risk { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class VehiclePredictionResponseDto
{
    public string Vehicle { get; set; } = string.Empty;
    public int HighRiskCount { get; set; }
    public int MediumRiskCount { get; set; }
    public int LowRiskCount { get; set; }
    public List<PartPredictionDto> Predictions { get; set; } = new();
}