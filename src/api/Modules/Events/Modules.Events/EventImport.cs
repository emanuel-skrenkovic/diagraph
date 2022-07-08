using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;

namespace Diagraph.Modules.Events;

public class EventImport
{
    private readonly EventsDbContext _context;

    public EventImport(EventsDbContext context) => _context = context;
    
    public async Task<int> ExecuteAsync(IEnumerable<Event> events, CancellationToken ct = default)
    {
        List<Event> eventsList = events.ToList();
        
        List<DateTime> dates = eventsList.Select(e => e.OccurredAtUtc).ToList();
        DateTime       start = dates.Min();
        DateTime       end   = dates.Max();

        List<Event> newEvents = 
        (
            await _context.ExceptByExistingAsync
            (
                eventsList,
                e => e.Discriminator,
                e => e.OccurredAtUtc >= start && e.OccurredAtUtc <= end
            )
        ).ToList();

        int rowsAffected = 0;
        if (newEvents.Any())
        {
            _context.AddRange(newEvents);
            rowsAffected = await _context.SaveChangesAsync(ct);
        }

        return rowsAffected;
    }
}