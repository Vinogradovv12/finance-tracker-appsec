using System.ComponentModel.DataAnnotations;
using FinanceTracker.Api.Common;
using FinanceTracker.Api.Data;
using FinanceTracker.Api.Data.Entities;
using FinanceTracker.Api.Exceptions;
using FinanceTracker.Api.Infrastructure.Auth.Interfaces;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IAppLogger _logger;
    private readonly IJwtProvider _provider;
    private readonly IPasswordHasher _passwordHasher;
    public AuthService(AppDbContext context, IAppLogger logger, IJwtProvider provider, IPasswordHasher passwordHasher)
    {
        _context = context;
        _logger = logger;
        _provider = provider;
        _passwordHasher = passwordHasher;
    }
    public async Task RegisterUserAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email) || email.Length > 100)
        {
            throw new ArgumentException("Invalid email address.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length > 128 || password.Length < 8)
        {
            throw new ArgumentException("Invalid password.", nameof(password));
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        bool userExists = await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail);
        
        if (userExists)
        {
            throw new UserAlreadyExistsException($"Attempt to register with {normalizedEmail} which already exists.");
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _logger.Info($"New user registered: {normalizedEmail}");
    }

    public async Task<string> LoginUserAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidCredentialsException("Email and password must be provided");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        bool isValid = user != null && _passwordHasher.VerifyPassword(password, user.PasswordHash);

        if (!isValid)
        {
            throw new InvalidCredentialsException($"Failed login attempt for {normalizedEmail} with provided password.");
        }

        var token = _provider.GenerateToken(user!);

        return token;
    }
}