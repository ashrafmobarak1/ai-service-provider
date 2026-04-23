namespace ServiceMarketplace.Application.RBAC.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentRoleId { get; set; }

    public IEnumerable<PermissionDto> Permissions { get; set; } = Enumerable.Empty<PermissionDto>();
}
