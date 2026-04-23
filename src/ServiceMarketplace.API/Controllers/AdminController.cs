using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketplace.API.Attributes;
using ServiceMarketplace.Application.Common.Interfaces;
using ServiceMarketplace.Application.RBAC.DTOs;
using ServiceMarketplace.Application.RBAC.Interfaces;

namespace ServiceMarketplace.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ICurrentUser _currentUser;

    public AdminController(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ICurrentUser currentUser)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _currentUser = currentUser;
    }

    // ── Roles ─────────────────────────────────────────────────────────────────

    /// <summary>List all roles with their permissions.</summary>
    [HttpGet("roles")]
    [RequiresPermission("role.manage")]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            ParentRoleId = r.ParentRoleId,
            Permissions = r.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Resource = rp.Permission.Resource,
                Action = rp.Permission.Action
            })
        }));
    }

    /// <summary>List all available permissions.</summary>
    [HttpGet("permissions")]
    [RequiresPermission("role.manage")]
    [ProducesResponseType(typeof(IEnumerable<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions()
    {
        var perms = await _permissionRepository.GetAllAsync();
        return Ok(perms.Select(p => new PermissionDto
        {
            Id = p.Id, Name = p.Name, Resource = p.Resource, Action = p.Action
        }));
    }

    // ── Role → Permission management ──────────────────────────────────────────

    /// <summary>Grant a permission to a role.</summary>
    [HttpPost("roles/{roleId:guid}/permissions/{permissionId:guid}")]
    [RequiresPermission("role.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GrantPermissionToRole(Guid roleId, Guid permissionId)
    {
        await _roleRepository.AddPermissionToRoleAsync(roleId, permissionId);
        return NoContent();
    }

    /// <summary>Revoke a permission from a role.</summary>
    [HttpDelete("roles/{roleId:guid}/permissions/{permissionId:guid}")]
    [RequiresPermission("role.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokePermissionFromRole(Guid roleId, Guid permissionId)
    {
        await _roleRepository.RemovePermissionFromRoleAsync(roleId, permissionId);
        return NoContent();
    }

    // ── User → Role management ────────────────────────────────────────────────

    /// <summary>Assign a role to a user.</summary>
    [HttpPost("users/{userId:guid}/roles/{roleId:guid}")]
    [RequiresPermission("user.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
    {
        await _roleRepository.AssignRoleToUserAsync(userId, roleId, _currentUser.Id);
        return NoContent();
    }

    /// <summary>Remove a role from a user.</summary>
    [HttpDelete("users/{userId:guid}/roles/{roleId:guid}")]
    [RequiresPermission("user.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
    {
        await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
        return NoContent();
    }

    // ── Direct user permission overrides ──────────────────────────────────────

    /// <summary>Directly grant a permission to a specific user (overrides role defaults).</summary>
    [HttpPost("users/{userId:guid}/permissions/{permissionId:guid}")]
    [RequiresPermission("role.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GrantDirectPermission(Guid userId, Guid permissionId)
    {
        await _permissionRepository.GrantDirectPermissionAsync(userId, permissionId, _currentUser.Id, isGranted: true);
        return NoContent();
    }

    /// <summary>Explicitly deny a permission for a specific user (highest priority).</summary>
    [HttpDelete("users/{userId:guid}/permissions/{permissionId:guid}")]
    [RequiresPermission("role.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DenyDirectPermission(Guid userId, Guid permissionId)
    {
        await _permissionRepository.GrantDirectPermissionAsync(userId, permissionId, _currentUser.Id, isGranted: false);
        return NoContent();
    }
}
