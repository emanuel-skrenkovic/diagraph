using System.Linq.Expressions;

namespace Diagraph.Infrastructure.Database.Extensions;

public static class DbContextExtensions
{
    public static IQueryable<T> WithUser<T>(this IQueryable<T> queryable, Guid userId) where T : IUserRelated
        => queryable.Where(e => e.UserId == userId);

    public static IQueryable<T> DistinctByField<T>(this IQueryable<T> queryable, Expression<Func<T, object>> selector) 
        => queryable
            .GroupBy(selector)
            .Select(e => e.FirstOrDefault());
}