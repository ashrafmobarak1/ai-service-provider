using ServiceMarketplace.Application.Auth.DTOs;
using ServiceMarketplace.Application.Auth.Interfaces;
using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.RBAC.Interfaces;
using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IRoleRepository _roleRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _roleRepository = roleRepository;
    }

    public async Task<Result<TokenDto>> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
            return Result<TokenDto>.Failure("User with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password)
        };

        await _userRepository.AddAsync(user);

        // Assign default role
        var customerRole = await _roleRepository.GetByNameAsync("Customer");
        if (customerRole != null)
        {
            await _roleRepository.AssignRoleToUserAsync(user.Id, customerRole.Id);
        }

        var roles = new[] { "Customer" };
        var token = _jwtService.GenerateToken(user, roles);

        return Result<TokenDto>.Success(new TokenDto
        {
            AccessToken = token,
            UserId = user.Id.ToString(),
            Email = user.Email,
            Subscription = user.Subscription.ToString(),
            Roles = roles
        });
    }

    public async Task<Result<TokenDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            return Result<TokenDto>.Failure("Invalid email or password.");

        var roles = await _userRepository.GetRoleNamesAsync(user.Id);
        var token = _jwtService.GenerateToken(user, roles);

        return Result<TokenDto>.Success(new TokenDto
        {
            AccessToken = token,
            UserId = user.Id.ToString(),
            Email = user.Email,
            Subscription = user.Subscription.ToString(),
            Roles = roles
        });
    }
}
