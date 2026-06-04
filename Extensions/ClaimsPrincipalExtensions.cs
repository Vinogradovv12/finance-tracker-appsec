using System.Security.Claims;

namespace FinanceTracker.Api.Extensions;

public static class ClaimsPrincipalExtension
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found in token");

        return Guid.Parse(userIdClaim.Value);
    }
}