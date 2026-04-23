using ServiceMarketplace.Application.RBAC.Interfaces;

namespace ServiceMarketplace.Application.RBAC.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    /// <summary>
    /// Resolution order:
    ///   1. Explicit deny in user_permissions  → DENY
    ///   2. Explicit grant in user_permissions → GRANT
    ///   3. Grant via role_permissions         → GRANT
    ///   4. Default                            → DENY
    /// </summary>
    public Task<bool> HasPermissionAsync(Guid userId, string permissionName)
        => throw new NotImplementedException();
}
