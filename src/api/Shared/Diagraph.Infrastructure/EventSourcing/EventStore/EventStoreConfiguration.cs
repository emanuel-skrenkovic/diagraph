namespace Diagraph.Infrastructure.EventSourcing.EventStore;

public class EventStoreConfiguration
{
    public const string SectionName = nameof(EventStoreConfiguration);
    
    public string ConnectionString { get; set; }
}