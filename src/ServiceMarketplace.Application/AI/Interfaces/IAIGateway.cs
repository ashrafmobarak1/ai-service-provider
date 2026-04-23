namespace ServiceMarketplace.Application.AI.Interfaces;

/// <summary>
/// Low-level gateway to the external AI provider (Claude API).
/// Implemented in Infrastructure. Application never imports the HTTP client directly.
/// </summary>
public interface IAIGateway
{
    Task<string> EnhanceTextAsync(string title, string description);
}
