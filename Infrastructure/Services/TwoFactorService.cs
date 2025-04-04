using Application.Services;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly IMemoryCache _cache;
    private readonly IEmailService _emailService;

    public TwoFactorService(IMemoryCache cache, IEmailService emailService)
    {
        _cache = cache;
        _emailService = emailService;
    }

    public async Task GenerateAndSendCodeAsync(AppUser user)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var cacheKey = GetCacheKey(user.Id);

        var twoFactorCode = new TwoFactorCode
        {
            UserId = user.Id,
            Code = code,
            Expiration = DateTime.UtcNow.AddMinutes(5)
        };

        _cache.Set(cacheKey, twoFactorCode, TimeSpan.FromMinutes(5));

        var subject = "Your BankingWebsite Verification Code";
        var body = $"Your 2FA verification code is: <b>{code}</b>. This code will expire in 5 minutes.";

        await _emailService.SendEmailAsync(user.Email!, subject, body);
    }

    public Task<bool> ValidateCodeAsync(AppUser user, string code)
    {
        var cacheKey = GetCacheKey(user.Id);

        if (_cache.TryGetValue(cacheKey, out var obj) && obj is TwoFactorCode savedCode)
        {
            if (savedCode.Code == code && savedCode.Expiration > DateTime.UtcNow)
            {
                _cache.Remove(cacheKey);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    private string GetCacheKey(string userId) => $"2fa-{userId}";
}
