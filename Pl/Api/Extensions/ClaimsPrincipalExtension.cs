using System;
using System.Security.Claims;

using static Common.Constants.IdentityServiceConstants;

namespace Api.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int GetApplicationUserId(this ClaimsPrincipal applicationUser)
    {
        var claim = applicationUser.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
        {
            throw new InvalidOperationException($"Claim '{ClaimTypes.NameIdentifier}' not found or empty");
        }
        if (!int.TryParse(claim.Value, out var applicationUserId))
        {
            throw new FormatException($"Claim '{ClaimTypes.NameIdentifier}' has invalid value: '{claim.Value}'");
        }

        return applicationUserId;
    }

    public static int GetUserId(this ClaimsPrincipal applicationUser)
    {
        var claim = applicationUser.FindFirst(CustomJwtClaimTypes.UserId);
        if (claim == null)
        {
            throw new InvalidOperationException($"Claim {CustomJwtClaimTypes.UserId} not found");
        }
        if (!int.TryParse(claim.Value, out var userId))
        {
            throw new FormatException($"Claim '{CustomJwtClaimTypes.UserId}' has invalid value: '{claim.Value}'");
        }

        return userId;
    }
}
