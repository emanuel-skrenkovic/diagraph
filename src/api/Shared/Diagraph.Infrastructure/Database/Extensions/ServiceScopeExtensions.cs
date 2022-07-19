using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Database.Extensions;

public static class ServiceScopeExtensions
{
    public static T GetContext<T>(this IServiceScope scope) where T : DbContext
        => scope.ServiceProvider.GetRequiredService<T>();
}