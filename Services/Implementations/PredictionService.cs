using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations;

 public class PredictionService : IPredictionService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<PredictionService> logger;
 
        private const string GroqApiUrl = "https://api.groq.com/openai/v1/chat/completions";
        private const string Model = "llama-3.3-70b-versatile";
 
        public PredictionService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<PredictionService> logger)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
        }
 
        public async Task<(bool Success, string Message, VehiclePredictionResponseDto? Result)>
            PredictPartFailuresAsync(VehiclePredictionRequestDto dto)
        {
            var apiKey = configuration["Groq:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return (false, "Groq API key is not configured.", null);
 
            var prompt = BuildPrompt(dto);
 
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.4,
                max_tokens = 1000
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
                return (false, "Failed to reach Groq API. Check your network connection.", null);
            }
 
            var responseJson = await response.Content.ReadAsStringAsync();
 
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Groq API error {Status}: {Body}", response.StatusCode, responseJson);
                return (false, $"Groq API returned an error: {response.StatusCode}", null);
            }
 
            List<PartPredictionDto> predictions;
            try
            {
                predictions = ParseGroqResponse(responseJson);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse Groq response: {Body}", responseJson);
                return (false, "Failed to parse AI response. Please try again.", null);
            }
 
            var result = new VehiclePredictionResponseDto
            {
                Vehicle = $"{dto.ManufacturingYear} {dto.Brand} {dto.Model} ({dto.FuelType})",
                HighRiskCount = predictions.Count(p => p.Risk == "High"),
                MediumRiskCount = predictions.Count(p => p.Risk == "Medium"),
                LowRiskCount = predictions.Count(p => p.Risk == "Low"),
                Predictions = predictions
                    .OrderBy(p => p.Risk switch
                    {
                        "High" => 0,
                        "Medium" => 1,
                        _ => 2
                    })
                    .ToList()
            };
 
            return (true, "Prediction successful.", result);
        }
 
        private static string BuildPrompt(VehiclePredictionRequestDto dto)
        {
            return $"""
                You are an expert automotive mechanic and parts advisor for a vehicle service center in Nepal.
 
                Analyse this vehicle and predict which parts are most likely to fail or need replacement soon,
                based on the details below.
 
                Vehicle: {dto.ManufacturingYear} {dto.Brand} {dto.Model} ({dto.FuelType})
                Total mileage: {dto.TotalMileageKm} km
                Monthly driving: {dto.MonthlyDrivingKm} km
                Road type: {dto.RoadType}
                Last service: {dto.MonthsSinceLastService} months ago
                Known symptoms: {(string.IsNullOrWhiteSpace(dto.KnownSymptoms) ? "None reported" : dto.KnownSymptoms)}
 
                Return a JSON array of 4-6 predicted part issues. Each object must have:
                - "part": name of the part (e.g. "Brake Pads", "Engine Oil", "Timing Belt")
                - "risk": "High", "Medium", or "Low"
                - "reason": 1-2 sentence explanation based on the vehicle data
                - "action": short recommended action (e.g. "Replace within 1,000 km")
 
                Respond ONLY with the raw JSON array. No markdown, no code fences, no extra text.
                """;
        }
 
        private static List<PartPredictionDto> ParseGroqResponse(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);
            var rawContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "[]";
            
            rawContent = rawContent
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
 
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
 
            return JsonSerializer.Deserialize<List<PartPredictionDto>>(rawContent, options)
                   ?? new List<PartPredictionDto>();
        }
    }