using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.RBAC.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task<IEnumerable<string>> GetRoleNamesAsync(Guid userId);
}
