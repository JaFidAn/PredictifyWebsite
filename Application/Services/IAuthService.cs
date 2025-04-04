using Application.Core;
using Application.DTOs.Auth;

namespace Application.Services;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(RegisterDto registerDto);
    Task<Result<string>> LoginAsync(LoginDto loginDto);
    Task<Result<string>> VerifyTwoFactorCodeAsync(Verify2FADto verifyDto);
    Task<Result<bool>> LogoutAsync();
    Task<Result<UserDto>> GetCurrentUserAsync();
}
