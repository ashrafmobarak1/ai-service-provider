using System.Security.Claims;
using ServiceMarketplace.Application.Common.Interfaces;

namespace ServiceMarketplace.API.Services;

/// <summary>
/// Reads the authenticated user's identity from the JWT claims in HttpContext.
/// Registered as Scoped so it captures the current request's ClaimsPrincipal.
/// </summary>
public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid Id
    {
        get
        {
            var sub = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? Principal?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    public string Email
        => Principal?.FindFirstValue(ClaimTypes.Email)
        ?? Principal?.FindFirstValue("email")
        ?? string.Empty;

    public IEnumerable<string> Roles
        => Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value)
        ?? Enumerable.Empty<string>();

    public bool IsAuthenticated
        => Principal?.Identity?.IsAuthenticated is true;
}
