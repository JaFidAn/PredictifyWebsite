namespace Application.DTOs.Auth;

public class Verify2FADto
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
