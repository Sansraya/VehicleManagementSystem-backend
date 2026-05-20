namespace VehicleManagementSystem.Dtos;

public class AdminRecommendationRequestDto
{
    // No body needed — backend pulls all data itself
}

public class PartOrderRecommendationDto
{
    public string PartName { get; set; } = string.Empty;
    public string PartCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int SuggestedOrderQuantity { get; set; }
    public string Urgency { get; set; } = string.Empty; // "Critical", "High", "Medium"
    public string Reason { get; set; } = string.Empty;
}

public class BusinessInsightDto
{
    public string Category { get; set; } = string.Empty; // "Revenue", "Operations", "Inventory", "Customer"
    public string Title { get; set; } = string.Empty;
    public string Insight { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // "High", "Medium", "Low"
}

public class AdminRecommendationResponseDto
{
    public string Summary { get; set; } = string.Empty;
    public List<PartOrderRecommendationDto> PartOrders { get; set; } = new();
    public List<BusinessInsightDto> BusinessInsights { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}