using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.RBAC.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);

    Task<Role?> GetByNameAsync(string name);

    Task<IEnumerable<Role>> GetAllAsync();

    Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid? assignedBy = null);

    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);

    Task AddPermissionToRoleAsync(Guid roleId, Guid permissionId);

    Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
}
