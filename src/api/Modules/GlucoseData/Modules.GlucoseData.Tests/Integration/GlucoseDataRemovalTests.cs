using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.Tests;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.GlucoseData.Database;
using Diagraph.Modules.GlucoseData.DataRemoval;
using Diagraph.Modules.GlucoseData.Imports;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Integration.UserData;
using Modules.Identity.Integration.UserData.Events;
using Xunit;

namespace Diagraph.Modules.GlucoseData.Tests.Integration;

[Collection(nameof(GlucoseDataCollectionFixture))]
public class GlucoseDataRemovalTests
{
    private readonly GlucoseDataFixture _fixture;

    public GlucoseDataRemovalTests(GlucoseDataFixture fixture) => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Removes_User_Events_On_RequestedEventDataRemovalEvent_Received
    (
        IEnumerable<GlucoseMeasurement> measurements
    )
    {
        // Arrange
        await _fixture.Module.ExecuteAsync<GlucoseDataDbContext>(async context =>
        {
            Import import = context.Add
            (
                new Import { Hash = "asdf", UserId = GlucoseDataFixture.RegisteredUserId }
            ).Entity;
            await context.SaveChangesAsync();

            context.GlucoseMeasurements.AddRange
            (
                measurements.Select(e =>
                {
                    e.UserId   = GlucoseDataFixture.RegisteredUserId;
                    e.ImportId = import.Id;
                    return e;
                })
            );
            await context.SaveChangesAsync();
        });

        AutoResetEvent handle = new AutoResetEvent(false);
        
        var listener = _fixture.Module.ServiceOfType<GlucoseDataRemovalListener, IEventSubscription>();
        await TestHandlerWrapper.RunAsync
        (
            _fixture.Module.Service<EventSubscriber>(),
            listener, 
            (_, _) => handle.Set()
        );
        
        // Act
        var requestedEventDataRemovalEvent = new RequestedGlucoseDataRemovalEvent
        (
            GlucoseDataFixture.RegisteredUserId.ToString()
        );
        await _fixture.EventStore.DispatchEvent
        (
            UserDataRemovalConsts.IntegrationStreamName, 
            requestedEventDataRemovalEvent
        );

        handle.WaitOne();
        
        // Assert
        var savedEvents = await _fixture
            .EventStore
            .Events(UserDataRemovalConsts.IntegrationStreamName);
        savedEvents.Should().NotBeNullOrEmpty();
        savedEvents.Should().ContainItemsAssignableTo<GlucoseDataRemovedEvent>();
        
        await _fixture.Module.ExecuteAsync<GlucoseDataDbContext>(async context =>
        {
            (await context.GlucoseMeasurements.AnyAsync()).Should().BeFalse();
        });
    } 
}