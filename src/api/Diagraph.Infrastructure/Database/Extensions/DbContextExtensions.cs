using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Database.Extensions;

public static class DbContextExtensions
{
    public static IQueryable<T> WithUser<T>(this IQueryable<T> query, Guid userId) 
        where T : IUserRelated
        => query.Where(e => e.UserId == userId);
}