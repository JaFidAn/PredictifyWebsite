using Application.Core;
using Application.DTOs.Auth;
using Application.Services;
using Application.Utilities;
using Application.Utulity;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITwoFactorService _twoFactorService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        ITwoFactorService twoFactorService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _twoFactorService = twoFactorService;
    }

    public async Task<Result<string>> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return Result<string>.Failure(MessageGenerator.AlreadyExists("User"), 400);
        }

        var user = new AppUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure(MessageGenerator.RegistrationFailed(errors), 400);
        }

        await _userManager.AddToRoleAsync(user, SD.Role_User);

        var token = await _tokenService.CreateToken(user);
        return Result<string>.Success(token, MessageGenerator.RegistrationSuccess());
    }

    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Result<string>.Failure(MessageGenerator.InvalidCredentials(), 401);

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Result<string>.Failure(MessageGenerator.InvalidCredentials(), 401);

        if (user.TwoFactorEnabled)
        {
            await _twoFactorService.GenerateAndSendCodeAsync(user);
            return Result<string>.Success(null!, "Verification code has been sent to your email");
        }

        var token = await _tokenService.CreateToken(user);
        return Result<string>.Success(token, "Login successful");
    }

    public async Task<Result<string>> VerifyTwoFactorCodeAsync(Verify2FADto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Result<string>.Failure(MessageGenerator.NotFound("User"), 404);

        var isValid = await _twoFactorService.ValidateCodeAsync(user, dto.Code);
        if (!isValid)
            return Result<string>.Failure("Invalid or expired verification code", 401);

        var token = await _tokenService.CreateToken(user);
        return Result<string>.Success(token, "Verification successful");
    }

    public async Task<Result<bool>> LogoutAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token))
            return Result<bool>.Failure("Token is required", 400);

        await _tokenService.RevokeTokenAsync(token);
        return Result<bool>.Success(true, MessageGenerator.LogoutSuccess());
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return Result<UserDto>.Failure(MessageGenerator.Unauthorized(), 401);

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<UserDto>.Failure(MessageGenerator.NotFound("User"), 404);

        var userDto = new UserDto
        {
            FullName = user.FullName,
            Email = user.Email!
        };

        return Result<UserDto>.Success(userDto);
    }
}
