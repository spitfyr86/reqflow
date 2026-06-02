using System.Security.Claims;
using ReqFlow.Application;

namespace ReqFlow.Api;

public static class AuthenticatedUserExtensions
{
    public static AuthenticatedUser GetAuthenticatedUser(this ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(subject, out var userId))
        {
            throw new ForbiddenException("The authenticated user identifier is invalid.");
        }

        return new AuthenticatedUser(userId);
    }
}
