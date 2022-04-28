namespace Diagraph.Infrastructure.Database.Extensions;

public static class DbContextExtensions
{
    public static IQueryable<T> WithUser<T>(this IQueryable<T> queryable, Guid userId) where T : IUserRelated
        => queryable.Where(e => e.UserId == userId);
}