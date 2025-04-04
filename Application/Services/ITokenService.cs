using Domain.Entities;

namespace Application.Services;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    Task RevokeTokenAsync(string token);
    Task<bool> IsTokenRevokedAsync(string token);
}
