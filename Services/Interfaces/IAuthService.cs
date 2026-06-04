using FinanceTracker.Api.Data.Entities;

namespace FinanceTracker.Api.Services.Interfaces;

public interface IAuthService
{
    public Task RegisterUserAsync(string email, string password);
    public Task<string> LoginUserAsync(string email, string password);
}