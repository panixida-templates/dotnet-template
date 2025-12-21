using Common.Enums;

namespace Common.SearchParams.Core;

public class BaseSearchParams
{
    private int _page = 1;
    public int Page
    {
        get { return _page; }
        set { _page = value < 1 ? 1 : value; }
    }
    public int? ObjectsCount { get; set; }
    public string? SortField { get; set; }
    public SortOrder SortOrder { get; set; }
    public string? SearchQuery { get; set; }
    public bool IsDeleted { get; set; } = false;

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public DateTime? UpdatedFrom { get; set; }
    public DateTime? UpdatedTo { get; set; }
    public DateTime? DeletedFrom { get; set; }
    public DateTime? DeletedTo { get; set; }

    public BaseSearchParams()
    {
        Page = 1;
    }
}
