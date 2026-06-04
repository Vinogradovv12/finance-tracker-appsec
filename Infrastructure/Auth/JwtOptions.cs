namespace FinanceTracker.Api.Infrastructure.Auth;

public class JwtOptions
{
    public string SecretKey { get; set; } = String.Empty;
    public int ExpiresHours { get; set; }
}
