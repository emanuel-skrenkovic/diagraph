using Diagraph.Core.Database;
using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Infrastructure.Database;

public class DiagraphDbContext : DbContextBase
{
    public DbSet<Meal> Meals { get; set; }
    public DbSet<InsulinApplication> InsulinApplications { get; set; }
    public DbSet<MiscellanousEvent> MiscellanousEvents { get; set; }
    
    public DbSet<Import> Imports { get; set; }
    public DbSet<GlucoseMeasurement> GlucoseMeasurements { get; set; }
    
    public DiagraphDbContext(DbContextOptions<DiagraphDbContext> dbContextOptions) 
        : base(dbContextOptions)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiagraphDbContext).Assembly);
}