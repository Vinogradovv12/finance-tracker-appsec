using System.ComponentModel.DataAnnotations;
using FinanceTracker.Api.Domain;

namespace FinanceTracker.Api.Data.Entities;
public class TransactionEntity : ITransaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    public TransactionsType Type { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserEntity User { get; set; } = null!;
}