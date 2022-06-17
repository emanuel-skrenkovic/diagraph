using System.Reflection;
using Diagraph.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Diagraph.Infrastructure.Database;

public class UserOwnedDbContext : DbContextBase
{
    private readonly IUserContext _userContext;
    
    protected UserOwnedDbContext
    (
        IUserContext userContext, 
        DbContextOptions dbContextOptions
    ) : base(dbContextOptions) => _userContext = userContext;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MethodInfo method = typeof(UserOwnedDbContext)
            .GetTypeInfo()
            .DeclaredMethods
            .Single(m => m.Name == nameof(Configure));
        
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            Type entityClrType = entityType.ClrType;
            if (typeof(IUserRelated).IsAssignableFrom(entityClrType))
            {
                method
                    .MakeGenericMethod(entityClrType)
                    .Invoke(this, new object[] { modelBuilder });
            }
        }
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry> entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IUserRelated &&
                        e.State is EntityState.Added);

        foreach (EntityEntry entry in entries)
        {
            var userOwnedEntity = (IUserRelated) entry.Entity;
            userOwnedEntity.UserId = _userContext.UserId;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }

    private void Configure<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        modelBuilder
            .Entity<TEntity>()
            .HasQueryFilter(e => ((IUserRelated)e).UserId == _userContext.UserId);
    }
}