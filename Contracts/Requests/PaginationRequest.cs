namespace FinanceTracker.Api.Contracts.Requests;

public record PaginationRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetItemsRequest : PaginationRequest
{
    public string? SearchTerm { get; init; }
}