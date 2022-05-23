using System.Security.Claims;
using Diagraph.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Modules.Identity.Api;

public static class UserContextMiddleware
{
    public static Task Handle(HttpContext context, Func<Task> next) 
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // lol
            UserContext userContext = context
                .RequestServices
                .GetRequiredService<IUserContext>() as UserContext;

            if (userContext is null) return next();
        
            userContext.UserId = Guid.Parse
            (
                context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            );
        }
    
        return next();
    }
}