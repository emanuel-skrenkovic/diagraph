using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.Tests;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataRemoval;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Integration.UserData;
using Modules.Identity.Integration.UserData.Events;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventDataRemovalTests
{
    private readonly EventsFixture _fixture;

    public EventDataRemovalTests(EventsFixture fixture) => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Removes_User_Events_On_RequestedEventDataRemovalEvent_Received
    (
        IEnumerable<Event> events
    )
    {
        // Arrange
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events.Select(e =>
            {
                e.UserId = EventsFixture.RegisteredUserId;
                return e;
            }));
            await context.SaveChangesAsync();
        });

        AutoResetEvent handle = new AutoResetEvent(false);
        
        var listener = _fixture.ServiceOfType<EventDataRemovalListener, IEventSubscription>();
        await TestHandlerWrapper.RunAsync
        (
            _fixture.Service<EventSubscriber>(),
            listener, 
            (_, _) => handle.Set()
        );
        
        // Act
        var requestedEventDataRemovalEvent = new RequestedEventDataRemovalEvent
        (
            EventsFixture.RegisteredUserId.ToString()
        );
        await _fixture.EventStoreFixture.DispatchEvent
        (
            UserDataRemovalConsts.IntegrationStreamName, 
            requestedEventDataRemovalEvent
        );

        handle.WaitOne();
        
        // Assert
        var savedEvents = await _fixture
            .EventStoreFixture
            .Events(UserDataRemovalConsts.IntegrationStreamName);
        savedEvents.Should().NotBeNullOrEmpty();
        savedEvents.Should().ContainItemsAssignableTo<EventDataRemovedEvent>();
        
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            (await context.Events.AnyAsync()).Should().BeFalse();
        });
    }
}