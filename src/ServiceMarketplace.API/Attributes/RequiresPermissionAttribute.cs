namespace ServiceMarketplace.API.Attributes;

/// <summary>
/// Marks a controller action with the permission name required to access it.
/// Enforced by <see cref="Filters.PermissionAuthorizationFilter"/> — not by ASP.NET Core's policy engine.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequiresPermissionAttribute : Attribute
{
    public string Permission { get; }

    public RequiresPermissionAttribute(string permission)
    {
        Permission = permission;
    }
}
