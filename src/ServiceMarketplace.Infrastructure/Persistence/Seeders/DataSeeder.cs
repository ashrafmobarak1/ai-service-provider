using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeds the database with default roles, permissions, and an admin user.
/// Called from Program.cs on startup — idempotent (checks before inserting).
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedPermissionsAsync(context);
        await SeedRolesAsync(context);
        await SeedAdminUserAsync(context);
        await SeedProviderUserAsync(context);
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        var permissions = new[]
        {
            ("request.create",   "request", "create",   "Create a service request"),
            ("request.view_own", "request", "view_own",  "View own service requests"),
            ("request.view_all", "request", "view_all",  "View all service requests"),
            ("request.accept",   "request", "accept",    "Accept a service request"),
            ("request.complete", "request", "complete",  "Mark a request as completed"),
            ("request.cancel",   "request", "cancel",    "Cancel a service request"),
            ("user.manage",      "user",    "manage",    "Manage users"),
            ("role.manage",      "role",    "manage",    "Assign/revoke roles and permissions"),
        };

        foreach (var (name, resource, action, desc) in permissions)
        {
            if (!await context.Permissions.AnyAsync(p => p.Name == name))
            {
                context.Permissions.Add(new Permission
                {
                    Name = name, Resource = resource, Action = action, Description = desc
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        var roleDefs = new[]
        {
            ("Admin",         "Platform administrator",               (string?)null),
            ("ProviderAdmin", "Provider with team management rights", (string?)null),
            ("Provider",      "Service provider",                     "ProviderAdmin"),
            ("Customer",      "Service customer",                     (string?)null),
        };

        foreach (var (name, desc, parentName) in roleDefs)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == name))
            {
                Guid? parentId = null;
                if (parentName is not null)
                    parentId = (await context.Roles.FirstOrDefaultAsync(r => r.Name == parentName))?.Id;

                context.Roles.Add(new Role { Name = name, Description = desc, ParentRoleId = parentId });
            }
        }

        await context.SaveChangesAsync();

        // Assign permissions to roles
        await AssignRolePermissionsAsync(context, "Admin",
            "request.create", "request.view_own", "request.view_all",
            "request.accept", "request.complete", "request.cancel",
            "user.manage", "role.manage");

        await AssignRolePermissionsAsync(context, "ProviderAdmin",
            "request.view_all", "request.accept", "request.complete", "request.view_own", "role.manage");

        await AssignRolePermissionsAsync(context, "Provider",
            "request.view_all", "request.accept", "request.complete", "request.view_own");

        await AssignRolePermissionsAsync(context, "Customer",
            "request.create", "request.view_own", "request.cancel");
    }

    private static async Task AssignRolePermissionsAsync(AppDbContext context, string roleName, params string[] permNames)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role is null) return;

        foreach (var permName in permNames)
        {
            var perm = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
            if (perm is null) continue;

            if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == perm.Id))
                context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = perm.Id });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(AppDbContext context)
    {
        const string adminEmail = "admin@marketplace.com";

        if (await context.Users.AnyAsync(u => u.Email == adminEmail))
            return;

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole is null) return;

        var admin = new User
        {
            Name = "System Admin",
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
            Subscription = SubscriptionTier.Paid,
            IsActive = true
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        context.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
        await context.SaveChangesAsync();
    }

    private static async Task SeedProviderUserAsync(AppDbContext context)
    {
        const string providerEmail = "provider@marketplace.com";

        if (await context.Users.AnyAsync(u => u.Email == providerEmail))
            return;

        var providerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Provider");
        if (providerRole is null) return;

        var provider = new User
        {
            Name = "Professional Provider",
            Email = providerEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Provider@123456"),
            Subscription = SubscriptionTier.Paid,
            IsActive = true
        };

        context.Users.Add(provider);
        await context.SaveChangesAsync();

        context.UserRoles.Add(new UserRole { UserId = provider.Id, RoleId = providerRole.Id });
        await context.SaveChangesAsync();
    }
}
