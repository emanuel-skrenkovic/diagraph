using Diagraph.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Diagraph.Modules.Identity.Database;

public class IdentityDbContext : DbContextBase
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> Profiles { get; set; }
    
    public IdentityDbContext(DbContextOptions<IdentityDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
}