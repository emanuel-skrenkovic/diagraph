using System;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.GlucoseData.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Diagraph.Modules.GlucoseData.Tests;

public class GlucoseDataFixture : DatabaseModuleFixture<GlucoseDataDbContext>, IAsyncLifetime
{
    public static readonly Guid RegisteredUserId = new("f0e7608f-e19a-450d-af66-ab9466f0a7fe"); 
    
    public EventStoreModuleFixture EventStoreFixture { get; }

    public GlucoseDataFixture() : base("glucose-data", ConfigureServices)
        => EventStoreFixture = new("glucose-data");

    private static void ConfigureServices(IServiceCollection services)
    {
        Mock<IUserContext> userContextMock = new();
        userContextMock.SetupGet(c => c.UserId).Returns(RegisteredUserId);
        services.AddScoped(_ => userContextMock.Object);
    }
    
    public new Task InitializeAsync() => Task.WhenAll
    (
        base.InitializeAsync(),
        EventStoreFixture.InitializeAsync()
    );

    public new Task DisposeAsync() => Task.WhenAll
    (
        base.DisposeAsync(),
        EventStoreFixture.DisposeAsync()
    );
}