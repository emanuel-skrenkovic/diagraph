using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Infrastructure.Database.Extensions;

public static class DbContextExtensions
{
    public static IQueryable<T> WithUser<T>(this IQueryable<T> queryable, Guid userId) 
        where T : IUserRelated
        => queryable.Where(e => e.UserId == userId);

    public static IQueryable<T> DistinctByField<T>
    (
        this IQueryable<T>          queryable, 
        Expression<Func<T, object>> selector
    ) => queryable
            .GroupBy(selector)
            .Select(e => e.FirstOrDefault());

    public static async Task<T> GetOrAddAsync<T>
    (
        this DbContext context, 
        Expression<Func<T, bool>> condition,
        T newValue
    )
        where T : class
    {
        T entity = await context
            .Set<T>()
            .FirstOrDefaultAsync(condition);

        if (entity is not null) return entity;
        
        context.Add(newValue);
        return newValue;
    }

    public static async Task<IEnumerable<T>> ExceptByExistingAsync<T, TDiscriminator>
    (
        this DbContext                      context,
        IEnumerable<T>                      entries,
        Expression<Func<T, TDiscriminator>> discriminatorSelector,
        Expression<Func<T, bool>>           filter = null
    ) where T : class
    {
        IQueryable<T> query = context.Set<T>();
        if (filter is not null) query = query.Where(filter);

        IEnumerable<TDiscriminator> discriminators = await query
            .Select(discriminatorSelector)
            .ToListAsync();

        return entries
            .ExceptBy(discriminators, discriminatorSelector.Compile())
            .ToList();
    }
}