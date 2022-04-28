using Diagraph.Infrastructure.Database;
using Diagraph.Modules.GlucoseData.Imports;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Diagraph.Modules.GlucoseData.Database;

public class GlucoseDataDbContext : DbContextBase
{
    public DbSet<Import> Imports { get; set; }
    public DbSet<GlucoseMeasurement> GlucoseMeasurements { get; set; } 
    
    public GlucoseDataDbContext(DbContextOptions<GlucoseDataDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(GlucoseDataDbContext).Assembly);
}