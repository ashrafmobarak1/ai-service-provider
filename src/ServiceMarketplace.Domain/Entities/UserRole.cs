namespace ServiceMarketplace.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    /// <summary>Id of the admin or manager who assigned this role.</summary>
    public Guid? AssignedBy { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
