using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Infrastructure.Persistence;

namespace ServiceMarketplace.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Role?> GetByIdAsync(Guid id)
        => _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

    public Task<Role?> GetByNameAsync(string name)
        => _context.Roles.FirstOrDefaultAsync(r => r.Name == name);

    public async Task<IEnumerable<Role>> GetAllAsync()
        => await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync();

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        var exists = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (!exists)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedBy = assignedBy
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole is not null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
        var exists = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

        if (!exists)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        var rp = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

        if (rp is not null)
        {
            _context.RolePermissions.Remove(rp);
            await _context.SaveChangesAsync();
        }
    }
}
