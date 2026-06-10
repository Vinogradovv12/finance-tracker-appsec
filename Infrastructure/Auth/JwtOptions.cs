namespace FinanceTracker.Api.Infrastructure.Auth;

public class JwtOptions
{
    public string SecretKey { get; set; } = String.Empty;
    public int ExpiresHours { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
