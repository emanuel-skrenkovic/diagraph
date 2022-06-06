using System.Text;

namespace Diagraph.Modules.Events.DataExports;

public class MergingDataExportStrategy : IDataExportStrategy
{
    private readonly IDataWriter _writer;

    public MergingDataExportStrategy(IDataWriter writer) => _writer = writer;
    
    // Everything about this seems pointless. Do it cleaner.
    public Task<byte[]> ExportAsync(IEnumerable<Event> events)
        => _writer.WriteEventAsync(MergeEvents(events));

    public Task<Stream> ExportStreamAsync(IEnumerable<Event> events)
        => _writer.WriteEventStreamAsync(MergeEvents(events));

    private IEnumerable<Event> MergeEvents(IEnumerable<Event> events)
    {
        var groupedEvents = events
            .OrderBy(e => e.OccurredAtUtc)
            .GroupBy
            (
                e => new EventGroupingData
                {
                    Tags          = e.Tags.Select(t => t.Name).ToArray(),
                    OccurredAtUtc = e.OccurredAtUtc
                }, 
                new EventTagsComparer()
            );

        return groupedEvents.Select
        (
            group =>
            {
                StringBuilder text = new();

                foreach (Event @event in group)
                    text.Append(@event.Text);

                return new Event
                {
                    OccurredAtUtc = group.First().OccurredAtUtc,
                    Text          = text.ToString()
                };
            }
        );        
    }

    private class EventGroupingData
    {
        public string[] Tags          { get; set; }
        public DateTime OccurredAtUtc { get; set; }
    }
    
    private class EventTagsComparer : IEqualityComparer<EventGroupingData>
    {
        public bool Equals(EventGroupingData x, EventGroupingData y)
        {
            if (x?.Tags is null || y?.Tags is null) return false;
            if (!x.Tags.Any() || !y.Tags.Any())     return false;
            
            return x.Tags.SequenceEqual(y.Tags) && x.OccurredAtUtc == y.OccurredAtUtc;
        }

        public int GetHashCode(EventGroupingData g)
        {
            return (string.Join(",", g.Tags) + $",{g.OccurredAtUtc}").GetHashCode();
        }
    }
}