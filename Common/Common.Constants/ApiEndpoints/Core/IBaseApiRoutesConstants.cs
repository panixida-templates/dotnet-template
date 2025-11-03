namespace Common.Constants.ApiEndpoints.Core;

public interface IBaseApiRoutesConstants
{
    const string BasePrefix = "api";
    const string GetByFilterSuffix = "get-by-filter";

    static abstract string ResourceName { get; }
    static abstract string Version { get; }
}
