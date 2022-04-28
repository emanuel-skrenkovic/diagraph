using System.Security.Claims;

namespace Diagraph.Modules.Identity.Api.Extensions;

public static class ClaimsIdentityExtensions
{
    public static ClaimsIdentity WithUser(this ClaimsIdentity identity, User user)
    {
        identity.AddClaim(new(ClaimTypes.Name, user.Email));
        identity.AddClaim(new(ClaimTypes.Email, user.Email));
        identity.AddClaim(new(ClaimTypes.NameIdentifier, user.Id.ToString()));

        return identity;
    }
}