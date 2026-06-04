using FinanceTracker.Api.Contracts.Requests;
using FinanceTracker.Api.Data.Entities;
using FinanceTracker.Api.Domain;

namespace FinanceTracker.Api.Services.Interfaces;
public interface ITransactionService
{
    public Task AddTransactionAsync(Guid userId, decimal amount, string category, TransactionsType type);
    public Task<(IEnumerable<TransactionEntity> Items, int TotalCount)> GetUserTransactionsAsync(Guid userId, GetItemsRequest request);
    public Task<decimal> GetBalanceAsync(Guid userId);
    public Task DeleteTransactionAsync(Guid transactionId, Guid userId);
}