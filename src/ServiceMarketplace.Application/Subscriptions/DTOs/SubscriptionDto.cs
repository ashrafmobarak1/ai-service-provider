namespace ServiceMarketplace.Application.Subscriptions.DTOs;

public class SubscriptionDto
{
    public string Tier { get; set; } = string.Empty;

    public int? MaxRequests { get; set; }

    public int ActiveRequestCount { get; set; }

    public bool IsLimitReached { get; set; }
}
