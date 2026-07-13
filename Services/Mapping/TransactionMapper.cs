using FinanceTracker.Api.Contracts.Response;
using FinanceTracker.Api.Domain;

namespace FinanceTracker.Api.Services.Mapping;

public static class TransactionMapper
{
    public static TransactionResponse ToResponse(ITransaction t)
    {
        return new TransactionResponse
        {
            Id = t.Id,
            Amount = t.Amount,
            Category = t.Category,
            Type = t.Type
        };
    }
}