namespace FinanceTracker.Api.Contracts.Response;

public record PagedResponse<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }

    public PagedResponse(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalRecords = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}