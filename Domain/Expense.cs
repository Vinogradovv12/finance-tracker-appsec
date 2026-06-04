namespace FinanceTracker.Api.Domain;

public class Expense : ITransaction
{
    public Guid Id { get; init; }
    public decimal Amount { get; private set; }
    public string Category { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public TransactionsType Type => TransactionsType.Expense;
    public decimal Value => -Amount;

    public Expense(Guid id, decimal amount, string category, Guid userId, DateTime createdAt)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be > 0");
        }

        if (amount > 1_000_000)
        {
            throw new ArgumentException("Amount too large");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(category, "Category is empty");

        if (category.Length > 50)
        {
            throw new ArgumentException("Category too large");
        }

        Id = id;
        Amount = amount;
        Category = category;
        UserId = userId;
        CreatedAt = createdAt;
    }
}