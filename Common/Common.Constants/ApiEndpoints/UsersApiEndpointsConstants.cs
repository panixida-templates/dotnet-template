using Common.Constants.ApiEndpoints.Core;

namespace Common.Constants.ApiEndpoints;

public sealed class UsersApiEndpointsConstants : IBaseApiEndpointsConstants<UsersApiEndpointsConstants, int>
{
    public const string BaseConstant = $"{IBaseApiRoutesConstants.BasePrefix}/{VersionConstant}/{ResourceNameConstant}";

    public const string ResourceNameConstant = "users";
    public const string ByIdConstant = "{id:int}";

    public const string VersionConstant = ApiVersionsConstants.V1;

    public static string ResourceName => ResourceNameConstant;
    public static string Version => VersionConstant;
}
