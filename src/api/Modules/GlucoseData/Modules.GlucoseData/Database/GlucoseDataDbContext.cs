using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database;
using Diagraph.Modules.GlucoseData.Imports;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Diagraph.Modules.GlucoseData.Database;

public class GlucoseDataDbContext : UserOwnedDbContext
{
    public DbSet<Import> Imports { get; set; }
    public DbSet<GlucoseMeasurement> GlucoseMeasurements { get; set; } 
    
    public GlucoseDataDbContext
    (
        IUserContext                           userContext, 
        DbContextOptions<GlucoseDataDbContext> dbContextOptions
    ) : base(userContext, dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GlucoseDataDbContext).Assembly);   
        base.OnModelCreating(modelBuilder);
    }
}