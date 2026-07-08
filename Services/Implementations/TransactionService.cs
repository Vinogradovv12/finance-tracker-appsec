using FinanceTracker.Api.Domain;
using FinanceTracker.Api.Common;
using FinanceTracker.Api.Services.Interfaces;
using FinanceTracker.Api.Data.Entities;
using FinanceTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Api.Contracts.Requests;

namespace FinanceTracker.Api.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _db;
    private readonly IAppLogger _logger;
    const int MAX_TRANSACTION = 10_000;

    public TransactionService(AppDbContext db, IAppLogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AddTransactionAsync(
        Guid userId,
        decimal amount,
        string category,
        TransactionsType type)
    {
        var count = await _db.Transactions
            .CountAsync(t => t.UserId == userId);

        if (count >= MAX_TRANSACTION)
            throw new InvalidOperationException("Transaction limit reached");

        if (amount <= 0 || amount > 1_000_000)
            throw new ArgumentException("Invalid amount");

        if (string.IsNullOrWhiteSpace(category) || category.Length > 50)
            throw new ArgumentException("Invalid category");

        var transaction = new TransactionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Category = category,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        _logger.Info($"Transaction added: {transaction.Id} for user {userId}");
    }

    public async Task<(IEnumerable<TransactionEntity> Items, int TotalCount)> GetUserTransactionsAsync(Guid userId, GetItemsRequest request)
    {
        var query =  _db.Transactions
            .Where(t => t.UserId == userId)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (items, totalCount);        
    }

    public async Task<decimal> GetBalanceAsync(Guid userId)
    {
        return await _db.Transactions
            .Where(t => t.UserId == userId)
            .SumAsync(t => t.Type == TransactionsType.Income
            ? t.Amount
            : -t.Amount);
    }

    public async Task DeleteTransactionAsync(Guid transactionId, Guid userId)
    {
        var transacton = await _db.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transacton == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        if (transacton.UserId != userId)
        {
            throw new UnauthorizedAccessException($"User {userId} attempted to delete someone else's transaction {transactionId}");
        }

        _db.Transactions.Remove(transacton);
        await _db.SaveChangesAsync();

         _logger.Info($"Transaction deleted: {transactionId} for user {userId}");
    }
}