namespace ServiceMarketplace.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }

    /// <summary>
    /// Dot-notation key. e.g. "request.create", "role.manage"
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public string Resource { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
