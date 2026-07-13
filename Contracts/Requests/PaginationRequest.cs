using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.Contracts.Requests;

public record PaginationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; init; } = 20;
}

public record GetItemsRequest : PaginationRequest
{
    public string? SearchTerm { get; init; }
}