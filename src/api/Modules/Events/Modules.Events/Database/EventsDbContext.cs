using Diagraph.Infrastructure.Database;
using Diagraph.Modules.Events.DataImports;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Diagraph.Modules.Events.Database;

public class EventsDbContext : DbContextBase
{
    public DbSet<Event> Events { get; set; }

    public DbSet<EventTag> EventTags { get; set; }
    public DbSet<ImportTemplate> Templates { get; set; }
    
    public EventsDbContext(DbContextOptions<EventsDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventsDbContext).Assembly);
}