using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations;

public class AdminRecommendationService : IAdminRecommendationService
{
    private readonly ApplicationDbContext dbContext;
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;
    private readonly ILogger<AdminRecommendationService> logger;

    private const string GroqApiUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string Model = "llama-3.3-70b-versatile";

    public AdminRecommendationService(
        ApplicationDbContext dbContext,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AdminRecommendationService> logger)
    {
        this.dbContext = dbContext;
        this.httpClient = httpClient;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<(bool Success, string Message, AdminRecommendationResponseDto? Result)>
        GetRecommendationsAsync()
    {
        var apiKey = configuration["Groq:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return (false, "Groq API key is not configured.", null);
        
        var parts = await dbContext.Parts.Include(p => p.Vendor).ToListAsync();

        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);
        var ninetyDaysAgo = now.AddDays(-90);

        var recentSales = await dbContext.Sales
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate >= thirtyDaysAgo)
            .ToListAsync();

        var allSales = await dbContext.Sales
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate >= ninetyDaysAgo)
            .ToListAsync();

        var appointments = await dbContext.Appointments
            .Where(a => a.AppointmentDate >= thirtyDaysAgo)
            .ToListAsync();

        var partRequests = await dbContext.PartRequests
            .Where(p => p.RequestDate >= thirtyDaysAgo)
            .ToListAsync();

        var vehicles = await dbContext.Vehicles.ToListAsync();
        
        var partSalesMap = allSales
            .SelectMany(s => s.SaleItems)
            .Where(si => si.PartsId != null)
            .GroupBy(si => si.PartsId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(si => si.Quantity));
        
        var partsSnapshot = parts.Select(p => new
        {
            name = p.Name,
            partCode = p.PartCode,
            category = p.Category,
            price = p.Price,
            stock = p.StockQuantity,
            soldLast90Days = partSalesMap.GetValueOrDefault(p.PartsId, 0),
            vendor = p.Vendor?.Name ?? "Unknown"
        }).ToList();

        var salesSnapshot = new
        {
            totalSalesLast30Days = recentSales.Count,
            totalRevenueLast30Days = recentSales.Sum(s => s.FinalAmount),
            totalDiscountLast30Days = recentSales.Sum(s => s.DiscountAmount),
            avgOrderValue = recentSales.Count > 0 ? recentSales.Average(s => s.FinalAmount) : 0,
            pendingPayments = recentSales.Count(s => s.Status == "Pending" ||
                (s.PaymentMethod?.ToLower() == "credit")),
            paymentMethods = recentSales
                .GroupBy(s => s.PaymentMethod ?? "Unknown")
                .Select(g => new { method = g.Key, count = g.Count() })
        };

        var appointmentsSnapshot = new
        {
            totalLast30Days = appointments.Count,
            byServiceType = appointments
                .GroupBy(a => a.ServiceType)
                .Select(g => new { type = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
        };

        var vehicleSnapshot = new
        {
            total = vehicles.Count,
            available = vehicles.Count(v => v.IsAvailable),
            sold = vehicles.Count(v => !v.IsAvailable)
        };

        var requestedParts = partRequests
            .GroupBy(r => r.PartName)
            .Select(g => new { partName = g.Key, requests = g.Count() })
            .OrderByDescending(x => x.requests)
            .Take(10)
            .ToList();

        var prompt = $$"""
    You are a senior business analyst for a vehicle management and parts service center in Nepal (Kathmandu).
    
    Analyze the following real business data and provide actionable recommendations.
    
    === PARTS INVENTORY ===
    {{JsonSerializer.Serialize(partsSnapshot, new JsonSerializerOptions { WriteIndented = false })}}
    
    === SALES (Last 30 days) ===
    {{JsonSerializer.Serialize(salesSnapshot, new JsonSerializerOptions { WriteIndented = false })}}
    
    === SERVICE APPOINTMENTS (Last 30 days) ===
    {{JsonSerializer.Serialize(appointmentsSnapshot, new JsonSerializerOptions { WriteIndented = false })}}
    
    === VEHICLE INVENTORY ===
    {{JsonSerializer.Serialize(vehicleSnapshot, new JsonSerializerOptions { WriteIndented = false })}}
    
    === CUSTOMER PART REQUESTS (Last 30 days) ===
    {{JsonSerializer.Serialize(requestedParts, new JsonSerializerOptions { WriteIndented = false })}}
    
    Return a JSON object with EXACTLY this structure:
    {
      "summary":"2-3 sentence executive summary of the business health and top priority",
      "partOrders":[
        {
          "partName": "string",
          "partCode": "string (from data, or 'N/A' if new part)",
          "category": "string",
          "currentStock": number,
          "suggestedOrderQuantity": number,
          "urgency": "Critical|High|Medium",
          "reason": "1 sentence explaining why"
        }
      ],
      "businessInsights": [
        {
          "category": "Revenue|Operations|Inventory|Customer",
          "title": "short title",
          "insight": "what the data shows",
          "action": "specific action to take",
          "priority": "High|Medium|Low"
        }
      ]
    }
    
    Rules:
    - partOrders: include parts with stock=0 (Critical), stock<5 (High), stock<10 AND sold frequently (Medium). Also include parts customers requested but don't exist in inventory. Suggest realistic order quantities based on sales velocity.
    - businessInsights: provide 4-6 diverse insights covering different categories. Be specific to Nepal market context.
    - Respond ONLY with the raw JSON. No markdown, no code fences, no explanation.
    """;
        var requestBody = new
        {
            model = Model,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.3,
            max_tokens = 2000
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(GroqApiUrl, content);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reach Groq API");
            return (false, "Failed to reach Groq API.", null);
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Groq API error {Status}: {Body}", response.StatusCode, responseJson);
            return (false, $"Groq API error: {response.StatusCode}", null);
        }

        try
        {
            using var doc = JsonDocument.Parse(responseJson);
            var rawContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "{}";

            rawContent = rawContent
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<AdminRecommendationResponseDto>(rawContent, options)
                         ?? new AdminRecommendationResponseDto();
            result.GeneratedAt = DateTime.UtcNow;

            return (true, "Recommendations generated.", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse Groq response");
            return (false, "Failed to parse AI response.", null);
        }
    }
}