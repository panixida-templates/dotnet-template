namespace Common.SearchParams.Core;

public sealed class SearchResult<T>
{
    public int Total { get; set; }
    public IList<T> Objects { get; set; } = [];
    public int RequestedPage { get; set; }
    public int? RequestedObjectsCount { get; set; }

    public SearchResult() { }

    public SearchResult(int total, IEnumerable<T> objects, int requestedPage, int? requestedObjectsCount)
    {
        Total = total;
        Objects = [.. objects];
        RequestedPage = requestedPage;
        RequestedObjectsCount = requestedObjectsCount;
    }

    public SearchResult<TOut> Map<TOut>(Func<IEnumerable<T>, IEnumerable<TOut>> selector)
    {
        return selector is null
            ? throw new ArgumentNullException(nameof(selector))
            : new SearchResult<TOut>(
            total: Total,
            objects: selector(Objects),
            requestedPage: RequestedPage,
            requestedObjectsCount: RequestedObjectsCount
        );
    }
}
