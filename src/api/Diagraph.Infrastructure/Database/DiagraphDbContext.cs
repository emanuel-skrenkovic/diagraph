using Diagraph.Core.Database;
using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Infrastructure.Database;

public class DiagraphDbContext : DbContextBase
{
    public DbSet<Event> Events { get; set; }
    public DbSet<Import> Imports { get; set; }
    public DbSet<GlucoseMeasurement> GlucoseMeasurements { get; set; }
    
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DiagraphDbContext(DbContextOptions<DiagraphDbContext> dbContextOptions) 
        : base(dbContextOptions)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiagraphDbContext).Assembly);
}