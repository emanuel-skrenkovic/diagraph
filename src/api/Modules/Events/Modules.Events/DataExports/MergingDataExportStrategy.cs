using System.Text;
using Diagraph.Modules.Events.DataExports.Contracts;

namespace Diagraph.Modules.Events.DataExports;

public class MergingDataExportStrategy : IDataExportStrategy
{
    public IEnumerable<Event> Run(IEnumerable<Event> events)
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
                    Text          = text.ToString(),
                    Tags          = group.First().Tags
                };
            }
        );        
    }

    private class EventGroupingData
    {
        public string[] Tags          { get; init; }
        public DateTime OccurredAtUtc { get; init; }
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