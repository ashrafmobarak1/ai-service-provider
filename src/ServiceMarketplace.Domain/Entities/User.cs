using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public SubscriptionTier Subscription { get; set; } = SubscriptionTier.Free;

    public Guid? EmployerId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? Employer { get; set; }

    public ICollection<User> Employees { get; set; } = new List<User>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public ICollection<ServiceRequest> CreatedRequests { get; set; } = new List<ServiceRequest>();

    public ICollection<ServiceRequest> AcceptedRequests { get; set; } = new List<ServiceRequest>();
}
