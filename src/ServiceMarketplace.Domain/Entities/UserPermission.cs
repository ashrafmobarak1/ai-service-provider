namespace ServiceMarketplace.Domain.Entities;

/// <summary>
/// Direct user-level permission override.
/// IsGranted = true  → explicit grant (overrides role defaults).
/// IsGranted = false → explicit deny  (highest priority, wins over role grants).
/// </summary>
public class UserPermission
{
    public Guid UserId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsGranted { get; set; } = true;

    public Guid? GrantedBy { get; set; }

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}
