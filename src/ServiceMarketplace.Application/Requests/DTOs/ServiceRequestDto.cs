using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Application.Requests.DTOs;

public class ServiceRequestDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? AiDescription { get; set; }

    public string Status { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public Guid? ProviderId { get; set; }

    public string? ProviderName { get; set; }

    public double? DistanceKm { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}
