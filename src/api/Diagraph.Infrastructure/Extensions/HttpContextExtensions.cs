using System.Security.Claims;
using Diagraph.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static Task SignInUserAsync(this HttpContext context, User user, string authScheme)
    {
        return context.SignInAsync
        (
            authScheme, 
            new ClaimsPrincipal
            (
                new ClaimsIdentity().WithUser(user)
            )
        );
    }
}