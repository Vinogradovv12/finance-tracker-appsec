namespace FinanceTracker.Api.Domain;

public enum TransactionsType { Income, Expense }
public interface ITransaction
{
    Guid Id { get; }
    decimal Amount { get; }
    string Category { get; }
    TransactionsType Type { get; }
    Guid UserId { get; }
    DateTime CreatedAt { get; }
}