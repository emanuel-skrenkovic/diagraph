using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Diagraph.Infrastructure.Database;

public class DbContextBase : DbContext
{
    protected DbContextBase(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry> entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is DbEntity &&
                        e.State is EntityState.Added or EntityState.Modified);

        DateTime utcNow = DateTime.UtcNow;
        foreach (EntityEntry entry in entries)
        {
            DbEntity entity = (DbEntity) entry.Entity;
            
            entity.UpdatedAtUtc = utcNow;
            
            if (EntityState.Added == entry.State) entity.CreatedAtUtc = utcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}