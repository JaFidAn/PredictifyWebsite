using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="dto">User registration data</param>
    /// <returns>JWT token if successful</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Login user and receive JWT token
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>JWT token if login is successful</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Verify two-factor authentication code
    /// </summary>
    /// <param name="dto">2FA code</param>
    /// <returns>JWT token if verification is successful</returns>
    [AllowAnonymous]
    [HttpPost("verify-2fa")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<IActionResult> Verify2FA(Verify2FADto dto)
    {
        var result = await _authService.VerifyTwoFactorCodeAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Logout current user and revoke token
    /// </summary>
    /// <returns>Logout success message</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync();
        return HandleResult(result);
    }

    /// <summary>
    /// Get information about the currently authenticated user
    /// </summary>
    /// <returns>User profile</returns>
    [HttpGet("current-user")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _authService.GetCurrentUserAsync();
        return HandleResult(result);
    }
}
