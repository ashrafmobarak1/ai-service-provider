namespace ServiceMarketplace.Application.RBAC.Interfaces;

public interface IPermissionService
{
    /// <summary>
    /// Checks whether the user holds the given permission,
    /// respecting explicit denies, direct grants, and role defaults.
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permissionName);
}
