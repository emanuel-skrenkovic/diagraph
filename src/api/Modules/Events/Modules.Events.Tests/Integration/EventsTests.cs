using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Infrastructure.Tests.Extensions;
using Diagraph.Modules.Events.Api.Events.Commands;
using Diagraph.Modules.Events.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventsTests
{
    private readonly EventsFixture _fixture;

    public EventsTests(EventsFixture fixture)
        => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Creates_Event(Event @event)
    {
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .PostAsJsonAsync("/events", @event);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        Uri location = response.Headers.Location;
        location.Should().NotBeNull();
        
        int id = location.AsId<int>();
        
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            Event newEvent = await context.FindAsync<Event>(id);
            newEvent.Should().NotBeNull();
        });
    }

    [Theory, CustomizedAutoData]
    public async Task Updates_Event(CreateEventCommand eventCreate, UpdateEventCommand eventUpdate)
    {
        // Arrange
        HttpResponseMessage insertResponse = await _fixture
            .Client
            .PostAsJsonAsync("/events", eventCreate);
        
        int id = insertResponse.Headers.Location.AsId<int>();
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .PutAsJsonAsync($"/events/{id}", eventUpdate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            Event updatedEvent = await context
                .Events
                .Include(nameof(Event.Tags))
                .FirstOrDefaultAsync(e => e.Id == id);
            
            updatedEvent.Should().NotBeNull();
            updatedEvent!.Text.Should().Be(eventUpdate.Text);

            foreach ((EventTag first, EventTag second) in updatedEvent
                         .Tags
                         .OrderBy(t => t.Name)
                         .Zip(eventUpdate.Tags.OrderBy(t => t.Name)))
            {
                first.Name.Should().Be(second.Name);
            }
        });
    }
}