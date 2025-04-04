using Domain.Entities;

namespace Application.Services;

public interface ITwoFactorService
{
    Task GenerateAndSendCodeAsync(AppUser user);
    Task<bool> ValidateCodeAsync(AppUser user, string code);
}
