using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.Auth.Interfaces;

public interface IJwtService
{
    /// <summary>Generates a signed JWT for the given user and their roles.</summary>
    string GenerateToken(User user, IEnumerable<string> roles);
}
