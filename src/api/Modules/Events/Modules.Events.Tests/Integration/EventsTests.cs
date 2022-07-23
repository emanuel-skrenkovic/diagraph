using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Infrastructure.Tests.Extensions;
using Diagraph.Modules.Events.Api.Contracts;
using Diagraph.Modules.Events.Api.Events.Contracts;
using Diagraph.Modules.Events.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventsTests
{
    private readonly EventsFixture _fixture;

    public EventsTests(EventsFixture fixture) => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Creates_Event(CreateEventCommand command)
    {
        // Arrange
        command.Notification = null;
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events", command);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        Uri location = response.Headers.Location;
        location.Should().NotBeNull();
        
        int id = location.AsId<int>();
        
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
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
            .Module
            .Client
            .PostAsJsonAsync("/events", eventCreate);
        
        int id = insertResponse.Headers.Location.AsId<int>();
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PutAsJsonAsync($"/events/{id}", eventUpdate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            Event updatedEvent = await context
                .Events
                .Include(nameof(Event.Tags))
                .FirstOrDefaultAsync(e => e.Id == id);
            
            updatedEvent.Should().NotBeNull();
            updatedEvent!.Text.Should().Be(eventUpdate.Text);

            foreach ((EventTag first, EventTagDto second) in updatedEvent
                         .Tags
                         .OrderBy(t => t.Name)
                         .Zip(eventUpdate.Tags.OrderBy(t => t.Name)))
            {
                first.Name.Should().Be(second.Name);
            }
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task Updates_Event_With_Added_EndedAtUtc
    (
        CreateEventCommand eventCreate, 
        UpdateEventCommand eventUpdate
    )
    {
        // Arrange
        HttpResponseMessage insertResponse = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events", eventCreate);
        
        int id = insertResponse.Headers.Location.AsId<int>();
        
        DateTime endedDate = DateTime.Now.AddHours(3);
        eventUpdate.EndedAtUtc = endedDate;
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PutAsJsonAsync($"/events/{id}", eventUpdate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            Event updatedEvent = await context
                .Events
                .Include(nameof(Event.Tags))
                .FirstOrDefaultAsync(e => e.Id == id);
            
            updatedEvent.Should().NotBeNull();
            updatedEvent!.Text.Should().Be(eventUpdate.Text);
            updatedEvent!.EndedAtUtc.Should().NotBeNull();

            DateTime eventEndedAt = updatedEvent!.EndedAtUtc!.Value;
            
            // TODO: create helper
            eventEndedAt.Year.Should().Be(endedDate.Year);
            eventEndedAt.Month.Should().Be(endedDate.Month);
            eventEndedAt.Day.Should().Be(endedDate.Day);
            eventEndedAt.Hour.Should().Be(endedDate.Hour);
            eventEndedAt.Minute.Should().Be(endedDate.Minute);
            eventEndedAt.Second.Should().Be(endedDate.Second);

            foreach ((EventTag first, EventTagDto second) in updatedEvent
                         .Tags
                         .OrderBy(t => t.Name)
                         .Zip(eventUpdate.Tags.OrderBy(t => t.Name)))
            {
                first.Name.Should().Be(second.Name);
            }
        });
    }
    
     [Theory, CustomizedAutoData]
     public async Task Deletes_Event(CreateEventCommand eventCreate)
     {
         // Arrange
         await _fixture.Database.CleanAsync();
         HttpResponseMessage insertResponse = await _fixture
             .Module
             .Client
             .PostAsJsonAsync("/events", eventCreate);
        
            int id = insertResponse.Headers.Location.AsId<int>();
        
         // Act
         HttpResponseMessage response = await _fixture
             .Module
             .Client
             .DeleteAsync($"/events/{id}");

         // Assert
         response.IsSuccessStatusCode.Should().BeTrue();
         response.StatusCode.Should().Be(HttpStatusCode.NoContent);

         await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
         { 
             (await context.Events.AnyAsync()).Should().BeFalse();
         }); 
     }
     
     [Theory, CustomizedAutoData]
     public async Task Delete_Event_Succeeds_After_Event_Deleted(CreateEventCommand eventCreate)
     {
         // Arrange
         await _fixture.Database.CleanAsync();
         HttpResponseMessage insertResponse = await _fixture
             .Module
             .Client
             .PostAsJsonAsync("/events", eventCreate);
        
         int id = insertResponse.Headers.Location.AsId<int>();
         
         await _fixture
             .Module
             .Client
             .DeleteAsync($"/events/{id}");
        
         // Act
         HttpResponseMessage response = await _fixture
             .Module
             .Client
             .DeleteAsync($"/events/{id}");

         // Assert
         response.IsSuccessStatusCode.Should().BeFalse();
         response.StatusCode.Should().Be(HttpStatusCode.NotFound);

         await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
         { 
             (await context.Events.AnyAsync()).Should().BeFalse();
         }); 
     }
}