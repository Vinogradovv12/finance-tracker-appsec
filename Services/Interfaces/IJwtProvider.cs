using FinanceTracker.Api.Data.Entities;

namespace FinanceTracker.Api.Services.Interfaces;

public interface IJwtProvider
{
    public string GenerateToken(UserEntity user);
}