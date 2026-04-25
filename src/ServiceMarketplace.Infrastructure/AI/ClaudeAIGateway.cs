using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ServiceMarketplace.Application.AI.Interfaces;

namespace ServiceMarketplace.Infrastructure.AI;

/// <summary>
/// Calls the Anthropic Claude API to enhance service request descriptions.
/// Uses HttpClient directly — no SDK dependency in Infrastructure.
/// </summary>
public class ClaudeAIGateway : IAIGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string? _apiKey;

    public ClaudeAIGateway(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _model = configuration["AI:Model"] ?? "claude-haiku-4-5-20251001";
        _apiKey = configuration["AI:ApiKey"];
    }

    public async Task<string> EnhanceTextAsync(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey == "MOCK")
        {
            // Simulating AI processing time
            await Task.Delay(1000); 
            return $"[AI MOCK ENHANCEMENT]\n" +
                   $"Service Title: {title}\n" +
                   $"Original Details: {description}\n\n" +
                   $"Refined Description: We need a professional service for '{title}'. " +
                   $"The client has specified: {description}. Please provide quotes based on these requirements.";
        }

        var prompt =
            $"You are a service marketplace assistant.\n" +
            $"Given a service request title and description, rewrite the description to be " +
            $"clear, professional, and helpful for service providers.\n" +
            $"Return ONLY the improved description — no preamble, no labels.\n\n" +
            $"Title: {title}\n" +
            $"Description: {description}";

        var requestBody = new
        {
            model = _model,
            max_tokens = 512,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/messages", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        return result
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? description;
    }
}
