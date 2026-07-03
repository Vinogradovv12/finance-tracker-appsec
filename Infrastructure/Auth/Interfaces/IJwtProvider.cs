using FinanceTracker.Api.Data.Entities;

namespace FinanceTracker.Api.Infrastructure.Auth.Interfaces;

public interface IJwtProvider
{
    public string GenerateToken(UserEntity user);
}