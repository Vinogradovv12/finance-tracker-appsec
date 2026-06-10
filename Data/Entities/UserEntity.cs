using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.Data.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;

    [MaxLength(72)]
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
}