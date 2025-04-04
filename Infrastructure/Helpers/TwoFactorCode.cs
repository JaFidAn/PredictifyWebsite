namespace Infrastructure.Helpers;

public class TwoFactorCode
{
    public string UserId { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTime Expiration { get; set; }
}
