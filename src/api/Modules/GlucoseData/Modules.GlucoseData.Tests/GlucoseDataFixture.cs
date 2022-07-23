using System;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.GlucoseData.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Diagraph.Modules.GlucoseData.Tests;

public class GlucoseDataFixture : IAsyncLifetime
{
    public static readonly Guid RegisteredUserId = new("f0e7608f-e19a-450d-af66-ab9466f0a7fe"); 
    
    public ModuleFixture Module { get; }
    public EventStoreFixture EventStore { get; }
    public DatabaseFixture<GlucoseDataDbContext> Database { get; }

    public GlucoseDataFixture()
    {
        Module     = new(ConfigureServices);
        EventStore = new();
        Database   = new("glucose-data", Module.Service<GlucoseDataDbContext>);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IUserContext> userContextMock = new();
        userContextMock.SetupGet(c => c.UserId).Returns(RegisteredUserId);
        services.AddScoped(_ => userContextMock.Object);
    }
    
    public Task InitializeAsync() => Task.WhenAll
    (
        Database.InitializeAsync(),
        EventStore.InitializeAsync()
    );

    public Task DisposeAsync() => Task.WhenAll
    (
        Database.DisposeAsync(),
        EventStore.DisposeAsync()
    );
}