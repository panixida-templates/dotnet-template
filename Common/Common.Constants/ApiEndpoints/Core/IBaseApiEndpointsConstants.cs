namespace Common.Constants.ApiEndpoints.Core;

public interface IBaseApiEndpointsConstants<TEndpoint, in TId> : IBaseApiRoutesConstants
    where TEndpoint : IBaseApiRoutesConstants, IBaseApiEndpointsConstants<TEndpoint, TId>
    where TId : notnull
{
    static virtual string Base() => $"{BasePrefix}/{TEndpoint.Version}/{TEndpoint.ResourceName}";
    static virtual string ById(TId id) => $"{TEndpoint.Base()}/{id}";
    static virtual string GetByFilter() => $"{TEndpoint.Base()}/{GetByFilterSuffix}";
}