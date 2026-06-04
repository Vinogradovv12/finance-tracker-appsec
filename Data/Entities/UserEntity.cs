using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.Data.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    [MaxLength(72)]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}