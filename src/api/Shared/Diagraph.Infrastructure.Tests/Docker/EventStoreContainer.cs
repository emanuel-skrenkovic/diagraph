using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Xunit;

namespace Diagraph.Infrastructure.Tests.Docker;

public class EventStoreContainer : IAsyncLifetime
{
    private readonly DockerContainer  _container;
    public           EventStoreClient EventStore;

    private readonly Func<EventStoreClient> _clientFactory;

    private const string ContainerName = "integration-tests.eventstore";
    private const string ImageName     = "eventstore/eventstore:21.10.5-bionic";
    
    public EventStoreContainer(string connectionString)
    {
        string port = new Uri(connectionString).Port.ToString();
        _container = new DockerContainer
        (
            ContainerName, 
            ImageName, 
            new()
            {
                "EVENTSTORE_CLUSTER_SIZE=1",
                "EVENTSTORE_RUN_PROJECTIONS=All",
                "EVENTSTORE_START_STANDARD_PROJECTIONS=true",
                "EVENTSTORE_EXT_TCP_PORT=1113",
                $"EVENTSTORE_HTTP_PORT={port}",
                "EVENTSTORE_INSECURE=true",
                "EVENTSTORE_ENABLE_EXTERNAL_TCP=true",
                "EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP=true"
            }, 
            new() { ["2113"] = port }
        )
        {
            CheckStatus = CheckConnection
        };

        _clientFactory = () => new EventStoreClient(EventStoreClientSettings.Create(connectionString));
    }
    
    private async Task<bool> CheckConnection()
    {
        EventStore = _clientFactory();
        if (EventStore == null) return false;

        try
        {
            await EventStore
                .ReadAllAsync(Direction.Forwards, Position.Start, maxCount: 1)
                .ToListAsync();
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task InitializeAsync() => _container.InitializeAsync();

    public async Task DisposeAsync()
    {
        if (EventStore != null) await EventStore.DisposeAsync();
        await _container.DisposeAsync();
    }
}