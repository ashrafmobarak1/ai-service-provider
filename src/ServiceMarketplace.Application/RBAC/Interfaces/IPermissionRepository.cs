using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.RBAC.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id);

    Task<Permission?> GetByNameAsync(string name);

    Task<IEnumerable<Permission>> GetAllAsync();

    /// <summary>
    /// Returns all permission names the user holds via roles plus direct grants,
    /// excluding explicit denies.
    /// </summary>
    Task<IEnumerable<string>> GetEffectivePermissionsAsync(Guid userId);

    Task GrantDirectPermissionAsync(Guid userId, Guid permissionId, Guid grantedBy, bool isGranted = true);

    Task RevokeDirectPermissionAsync(Guid userId, Guid permissionId);
}
