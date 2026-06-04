using FinanceTracker.Api.Domain;

namespace FinanceTracker.Api.Contracts.Response;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public TransactionsType Type { get; set; }
}