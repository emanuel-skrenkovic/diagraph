using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.Events.Database;
using FluentAssertions;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventsDataExportTests
{
    private readonly EventsFixture _fixture;

    public EventsDataExportTests(EventsFixture fixture)
        => _fixture = fixture;

    [Fact]
    public async Task Returns_Ok_When_No_Events()
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string content = await response.Content.ReadAsStringAsync();
        content.Should().BeNullOrWhiteSpace();
    }
    
    [Theory, CustomizedAutoData]
    public async Task Exports_Events_Individually(List<Event> events)
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();
        
        events.ToList().ForEach(e => e.UserId = EventsFixture.RegisteredUserId);
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        Stream             content      = await response.Content.ReadAsStreamAsync();
        IEnumerable<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count().Should().Be(events.Count);
    }
    
    [Theory, CustomizedAutoData]
    public async Task Exports_Events_Individually_With_MergeSequential_Query_Parameter_Specified
    (
        List<Event> events
    )
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();
        events.ToList().ForEach(e => e.UserId = EventsFixture.RegisteredUserId);
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });

        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv?mergeSequential=false");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Stream             content      = await response.Content.ReadAsStreamAsync();
        IEnumerable<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count().Should().Be(events.Count);
    }
    
    [Fact]
    public async Task Exports_Events_Merged()
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();

        List<Event> events = new()
        {
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text1",
                Tags          = new[] { new EventTag { Name = "tag1" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text2",
                Tags          = new[] { new EventTag { Name = "tag1" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-02").ToUniversalTime(),
                Text          = "text3",
                Tags          = new[] { new EventTag { Name = "tag2" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            }
        };
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv?mergeSequential=true");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Stream             content      = await response.Content.ReadAsStreamAsync();
        List<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().BeLessThan(events.Count);
        parsedEvents.Count.Should().Be(2);

        parsedEvents[0].Text
            .Should()
            .Be(events[0].Text + events[1].Text);
    }
    
    [Fact]
    public async Task Does_Not_Merge_Events_When_Tags_Missing()
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();

        List<Event> events = new()
        {
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text1",
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text2",
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-02").ToUniversalTime(),
                Text          = "text3",
                Tags          = new[] { new EventTag { Name = "tag2" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            }
        };
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv?mergeSequential=true");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Stream             content      = await response.Content.ReadAsStreamAsync();
        List<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(events.Count);
    }
    
    [Fact]
    public async Task Exports_Events_Merged_When_Tags_Missing()
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();

        List<Event> events = new()
        {
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text1",
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text2",
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-02").ToUniversalTime(),
                Text          = "text3",
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            }
        };
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv?mergeSequential=true");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Stream             content      = await response.Content.ReadAsStreamAsync();
        List<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(events.Count);
    }
    
    [Fact]
    public async Task Does_Not_Merge_Events_With_Same_Date_And_Different_Tags()
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();
        List<Event> events = new()
        {
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text1",
                Tags          = new[] { new EventTag { Name = "tag1" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text2",
                Tags          = new[] { new EventTag { Name = "tag2" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-02").ToUniversalTime(),
                Text          = "text3",
                Tags          = new[] { new EventTag { Name = "tag3" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            }
        };
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv?mergeSequential=true");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Stream             content      = await response.Content.ReadAsStreamAsync();
        List<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(events.Count);
    }
    
    [Fact]
    public async Task Does_Not_Merge_Events_With_Same_Tags_And_Different_Dates()
    {
        // Arrange
        await _fixture.Postgres.CleanAsync();

        List<Event> events = new()
        {
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-01").ToUniversalTime(),
                Text          = "text1",
                Tags          = new[] { new EventTag { Name = "tag" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-02").ToUniversalTime(),
                Text          = "text2",
                Tags          = new[] { new EventTag { Name = "tag" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            },
            new()
            {
                OccurredAtUtc = DateTime.Parse("2020-01-03").ToUniversalTime(),
                Text          = "text3",
                Tags          = new[] { new EventTag { Name = "tag" } },
                UserId        = EventsFixture.RegisteredUserId,
                Source        = "custom"
            }
        };
        await _fixture.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Client
            .GetAsync("events/data-export/csv?mergeSequential=true");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Stream      content      = await response.Content.ReadAsStreamAsync();
        List<Event> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(events.Count);
    }
    
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        Delimiter       = ",",
        HasHeaderRecord = true
    };

    private IEnumerable<Event> ParseEventsCsv(Stream data)
    {
        using StreamReader reader = new(data);
        using CsvReader    csv    = new(reader, Configuration);

        List<Event> events = new();

        while (csv.Read())
            events.Add(csv.GetRecord<Event>());

        return events;
    }
}