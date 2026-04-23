using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceMarketplace.API.Attributes;
using ServiceMarketplace.Application.Common.Interfaces;
using ServiceMarketplace.Application.RBAC.Interfaces;

namespace ServiceMarketplace.API.Filters;

/// <summary>
/// Action filter that reads [RequiresPermission] from the route and calls
/// IPermissionService to enforce it before the controller body runs.
/// Returns 401 if unauthenticated, 403 if the permission is not held.
/// </summary>
public class PermissionAuthorizationFilter : IAsyncActionFilter
{
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    public PermissionAuthorizationFilter(
        IPermissionService permissionService,
        ICurrentUser currentUser)
    {
        _permissionService = permissionService;
        _currentUser = currentUser;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<RequiresPermissionAttribute>()
            .FirstOrDefault();

        // No attribute → no permission check required
        if (attribute is null)
        {
            await next();
            return;
        }

        if (!_currentUser.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var hasPermission = await _permissionService
            .HasPermissionAsync(_currentUser.Id, attribute.Permission);

        if (!hasPermission)
        {
            context.Result = new ObjectResult(new { error = $"Permission '{attribute.Permission}' is required." })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        await next();
    }
}
