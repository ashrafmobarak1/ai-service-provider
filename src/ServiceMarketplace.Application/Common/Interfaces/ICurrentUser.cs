namespace ServiceMarketplace.Application.Common.Interfaces;

/// <summary>
/// Provides the identity of the authenticated user for the current request.
/// Implemented in the API layer via HttpContext.
/// </summary>
public interface ICurrentUser
{
    Guid Id { get; }

    string Email { get; }

    IEnumerable<string> Roles { get; }

    bool IsAuthenticated { get; }
}
