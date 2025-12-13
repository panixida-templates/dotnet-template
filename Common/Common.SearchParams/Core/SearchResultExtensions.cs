namespace Common.SearchParams.Core;

public static class SearchResultExtensions
{
    public static SearchResult<TOut> Map<TIn, TOut>(this SearchResult<TIn> source, Func<TIn, TOut> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return new SearchResult<TOut>(
            total: source.Total,
            objects: source.Objects.Select(selector),
            requestedPage: source.RequestedPage,
            requestedObjectsCount: source.RequestedObjectsCount
        );
    }
}
