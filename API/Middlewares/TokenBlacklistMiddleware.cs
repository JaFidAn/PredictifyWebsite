using Application.Services;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public TokenBlacklistMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

                if (await tokenService.IsTokenRevokedAsync(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token is revoked");
                    return;
                }
            }
        }

        await _next(context);
    }
}