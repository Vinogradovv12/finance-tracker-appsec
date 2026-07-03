using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using FinanceTracker.Api.Infrastructure.Auth.Interfaces;
using Microsoft.Extensions.Options;

namespace FinanceTracker.Api.Infrastructure.Auth.Implementations;

public class PasswordHasher : IPasswordHasher
{
    private readonly Argon2Options _options;
    private const string FormatPrefix = "$argon2id$v=19";

    public PasswordHasher(IOptions<Argon2Options> options)
    {
        _options = options.Value;
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) 
            throw new ArgumentNullException(nameof(password));

        byte[] salt = new byte[_options.SaltSize];
        RandomNumberGenerator.Fill(salt);

        byte[] hash = ComputeArgon2(password, salt, _options.MemorySize, _options.Iterations, _options.DegreeOfParallelism, _options.HashSize);

        string saltBase64 = Convert.ToBase64String(salt).TrimEnd('=');
        string hashBase64 = Convert.ToBase64String(hash).TrimEnd('=');

        return $"{FormatPrefix}m={_options.MemorySize},t={_options.Iterations},p={_options.DegreeOfParallelism}${saltBase64}${hashBase64}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword)) 
            return false;

        if (!hashedPassword.StartsWith(FormatPrefix)) 
            return false;

        try
        {
            var parts = hashedPassword.Replace(FormatPrefix, "").Split('$');
            if (parts.Length != 3) return false;

            var paramsPart = parts[0].Split(',');
            int memory = int.Parse(paramsPart[0].Replace("m=", ""));
            int iterations = int.Parse(paramsPart[1].Replace("t=", ""));
            int parallelism = int.Parse(paramsPart[2].Replace("p=", ""));

            byte[] salt = ConvertFromBase64WithPadding(parts[1]);
            byte[] expectedHash = ConvertFromBase64WithPadding(parts[2]);

            byte[] actualHash = ComputeArgon2(password, salt, memory, iterations, parallelism, expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }

    private byte[] ComputeArgon2(string password, byte[] salt, int memory, int iterations, int parallelism, int hashSize)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        
        argon2.Salt = salt;
        argon2.MemorySize = memory;
        argon2.Iterations = iterations;
        argon2.DegreeOfParallelism = parallelism;

        return argon2.GetBytes(hashSize);
    }

    private byte[] ConvertFromBase64WithPadding(string base64)
    {
        int paddingNeeded = (4 - (base64.Length % 4)) % 4;
        if (paddingNeeded > 0)
        {
            base64 += new string('=', paddingNeeded);
        }
        return Convert.FromBase64String(base64);
    }
}
