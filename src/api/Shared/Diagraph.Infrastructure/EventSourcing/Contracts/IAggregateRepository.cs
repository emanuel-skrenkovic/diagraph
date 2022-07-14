namespace Diagraph.Infrastructure.EventSourcing.Contracts;

public interface IAggregateRepository
{
    Task SaveAsync<T, TKey>(T aggregate) where T : AggregateEntity<TKey>;
    
    Task<T> LoadAsync<T, TKey>(TKey key) where T : AggregateEntity<TKey>, new();
}