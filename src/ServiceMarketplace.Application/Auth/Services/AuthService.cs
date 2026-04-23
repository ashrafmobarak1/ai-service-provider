using ServiceMarketplace.Application.Auth.DTOs;
using ServiceMarketplace.Application.Auth.Interfaces;
using ServiceMarketplace.Application.Common;
using ServiceMarketplace.Application.RBAC.Interfaces;

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

    public Task<Result<TokenDto>> RegisterAsync(RegisterDto dto)
        => throw new NotImplementedException();

    public Task<Result<TokenDto>> LoginAsync(LoginDto dto)
        => throw new NotImplementedException();
}
