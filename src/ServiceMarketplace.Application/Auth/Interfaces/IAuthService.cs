using ServiceMarketplace.Application.Auth.DTOs;
using ServiceMarketplace.Application.Common;

namespace ServiceMarketplace.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<Result<TokenDto>> RegisterAsync(RegisterDto dto);

    Task<Result<TokenDto>> LoginAsync(LoginDto dto);
}
