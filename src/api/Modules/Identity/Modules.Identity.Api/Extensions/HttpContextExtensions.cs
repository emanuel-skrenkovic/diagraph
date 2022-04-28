using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Modules.Identity.Api.Extensions;

public static class HttpContextExtensions
{
    public static Task SignInUserAsync(this HttpContext context, User user, string authScheme)
    {
        return context.SignInAsync
        (
            authScheme, 
            new ClaimsPrincipal
            (
                new ClaimsIdentity(authScheme).WithUser(user)
            )
        );
    }
}