using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Domain.Entities;

public class ServiceRequest
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Populated after AI enhancement. Null until enhanced.</summary>
    public string? AiDescription { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    // Geolocation
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    // Relationships
    public Guid CustomerId { get; set; }

    public Guid? ProviderId { get; set; }

    // Lifecycle timestamps
    public DateTime? AcceptedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User Customer { get; set; } = null!;

    public User? Provider { get; set; }
}
