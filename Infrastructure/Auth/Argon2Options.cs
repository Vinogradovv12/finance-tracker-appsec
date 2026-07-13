namespace FinanceTracker.Api.Infrastructure.Auth;

public class Argon2Options
{
    public int SaltSize { get; set; } = 16;
    public int DegreeOfParallelism { get; set; } = 4;
    public int Iterations { get; set; } = 3;
    public int MemorySize { get; set; } = 65536;
    public int HashSize { get; set; } = 32;
}