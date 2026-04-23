namespace ServiceMarketplace.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Enables role hierarchy. e.g. Provider.ParentRoleId = ProviderAdmin.Id
    /// </summary>
    public Guid? ParentRoleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Role? ParentRole { get; set; }

    public ICollection<Role> ChildRoles { get; set; } = new List<Role>();

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
