using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database;
using Diagraph.Modules.Events.DataImports;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Diagraph.Modules.Events.Database;

public class EventsDbContext : UserOwnedDbContext
{
    public DbSet<Event> Events { get; set; }

    public DbSet<EventTag> EventTags { get; set; }
    public DbSet<ImportTemplate> Templates { get; set; }
    
    public EventsDbContext
    (
        IUserContext                      userContext, 
        DbContextOptions<EventsDbContext> dbContextOptions
    ) : base(userContext, dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}