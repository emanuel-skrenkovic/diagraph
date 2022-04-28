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
            UserContext userContext = (UserContext) context
                .RequestServices
                .GetRequiredService<IUserContext>();
        
            userContext.UserId = Guid.Parse
            (
                context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            );
        }
    
        return next();
    }
}