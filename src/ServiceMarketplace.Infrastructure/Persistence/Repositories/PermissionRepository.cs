using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Infrastructure.Persistence;

namespace ServiceMarketplace.Infrastructure.Persistence.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Permission?> GetByIdAsync(Guid id)
        => _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);

    public Task<Permission?> GetByNameAsync(string name)
        => _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);

    public async Task<IEnumerable<Permission>> GetAllAsync()
        => await _context.Permissions.ToListAsync();

    /// <summary>
    /// Resolves effective permissions for a user:
    ///   - Collects explicit denies from user_permissions (is_granted = false)
    ///   - Collects direct grants from user_permissions (is_granted = true)
    ///   - Collects role-based grants via user_roles → role_permissions
    ///   - Returns names that are granted and NOT denied
    /// </summary>
    public async Task<IEnumerable<string>> GetEffectivePermissionsAsync(Guid userId)
    {
        // Explicit denies
        var deniedNames = await _context.UserPermissions
            .Where(up => up.UserId == userId && !up.IsGranted)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        // Direct grants
        var directGrants = await _context.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        // Get initial roles assigned to the user
        var userRoleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // Recursively find all descendant roles (child roles)
        var allRoleIds = new HashSet<Guid>(userRoleIds);
        var rolesToProcess = new Queue<Guid>(userRoleIds);

        while (rolesToProcess.Count > 0)
        {
            var parentId = rolesToProcess.Dequeue();
            var children = await _context.Roles
                .Where(r => r.ParentRoleId == parentId)
                .Select(r => r.Id)
                .ToListAsync();

            foreach (var childId in children)
            {
                if (allRoleIds.Add(childId))
                {
                    rolesToProcess.Enqueue(childId);
                }
            }
        }

        // Collect permissions from all identified roles
        var roleGrants = await _context.RolePermissions
            .Where(rp => allRoleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return directGrants
            .Union(roleGrants)
            .Except(deniedNames)
            .Distinct();
    }

    public async Task GrantDirectPermissionAsync(
        Guid userId, Guid permissionId, Guid grantedBy, bool isGranted = true)
    {
        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existing is not null)
        {
            existing.IsGranted = isGranted;
            existing.GrantedBy = grantedBy;
            existing.GrantedAt = DateTime.UtcNow;
        }
        else
        {
            _context.UserPermissions.Add(new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsGranted = isGranted,
                GrantedBy = grantedBy
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokeDirectPermissionAsync(Guid userId, Guid permissionId)
    {
        var up = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (up is not null)
        {
            _context.UserPermissions.Remove(up);
            await _context.SaveChangesAsync();
        }
    }
}
