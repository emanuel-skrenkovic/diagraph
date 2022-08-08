using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports.Csv;
using FluentAssertions;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventsDataExportTests : IAsyncLifetime
{
    private readonly EventsFixture _fixture;

    public EventsDataExportTests(EventsFixture fixture)
        => _fixture = fixture;

    [Fact]
    public async Task Returns_BadRequest_When_Template_Query_Parameter_Not_Provided()
    {
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync("events/data-export/csv");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory, CustomizedAutoData]
    public async Task Returns_Ok_When_No_Events(CreateExportTemplateCommand createExportTemplate)
    {
        // Arrange
        await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/export-templates", createExportTemplate); 
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?template={createExportTemplate.Name}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string content = await response.Content.ReadAsStringAsync();
        content.Should().BeNullOrWhiteSpace();
    }
    
    [Theory, CustomizedAutoData]
    public async Task Exports_Events_Individually(string templateName, List<Event> events)
    {
        // Arrange
        await CreateTemplateFromEventsAsync(templateName, events);
        
        // Arrange
        events.ToList().ForEach(e => e.UserId = EventsFixture.RegisteredUserId);
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        Stream content = await response.Content.ReadAsStreamAsync();
        IEnumerable<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count().Should().Be(events.Count);
    }
    
    [Theory, CustomizedAutoData]
    public async Task Exports_Events_Individually_With_MergeSequential_Query_Parameter_Specified
    (
        string      templateName,
        List<Event> events
    )
    {
        // Arrange
        await CreateTemplateFromEventsAsync(templateName, events);
        
        events.ToList().ForEach(e => e.UserId = EventsFixture.RegisteredUserId);
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
    
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?mergeSequential=false&template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        Stream content = await response.Content.ReadAsStreamAsync();
        IEnumerable<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count().Should().Be(events.Count);
    }
    
    [Fact]
    public async Task Exports_Events_Merged()
    {
        // Arrange
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
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });

        const string templateName = "test-template";
        await CreateTemplateFromEventsAsync(templateName, events);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?mergeSequential=true&template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        Stream content = await response.Content.ReadAsStreamAsync();
        List<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().BeLessThan(events.Count);
        parsedEvents.Count.Should().Be(2);
    
        // Need to cast to string because of dynamic fuckery.
        ((string) parsedEvents[0].tag1)
            .Should()
            .Be(events[0].Text + events[1].Text);
    }
    
    [Fact]
    public async Task Does_Not_Output_Events_When_Tags_Missing()
    {
        // Arrange
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
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });
        
        const string templateName = "test-template";
        await CreateTemplateFromEventsAsync(templateName, events);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?mergeSequential=true&template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        Stream content = await response.Content.ReadAsStreamAsync();
        List<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(events.Count(e => e.Tags?.Any() == true));
    }
    
    [Fact]
    public async Task Does_Not_Export_Events_Merged_When_Tags_Missing()
    {
        // Arrange
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
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });

        const string templateName = "test-template";
        await CreateTemplateFromEventsAsync(templateName, events);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?mergeSequential=true&template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        Stream content = await response.Content.ReadAsStreamAsync();
        List<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Does_Not_Merge_Events_With_Same_Date_And_Different_Tags()
    {
        // Arrange
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
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });

        const string templateName = "test-template";
        await CreateTemplateFromEventsAsync(templateName, events);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?mergeSequential=true&template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        Stream             content      = await response.Content.ReadAsStreamAsync();
        List<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(2);
        
        ((string) parsedEvents[0].tag1)
                    .Should()
                    .NotBe(events[0].Text + events[1].Text);
    }
    
    [Fact]
    public async Task Does_Not_Merge_Events_With_Same_Tags_And_Different_Dates()
    {
        // Arrange
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
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            context.AddRange(events);
            await context.SaveChangesAsync();
        });

        const string templateName = "test-template";
        await CreateTemplateFromEventsAsync(templateName, events);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .GetAsync($"events/data-export/csv?mergeSequential=true&template={templateName}");
         
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        Stream content = await response.Content.ReadAsStreamAsync();
        List<dynamic> parsedEvents = ParseEventsCsv(content).ToList();
        parsedEvents.Should().NotBeNull();
        parsedEvents.Should().NotBeEmpty();
        parsedEvents.Count.Should().Be(events.Count);
        
        ((string) parsedEvents[0].tag)
            .Should()
            .NotBe(events[0].Text + events[1].Text);
    }

    private async Task CreateTemplateFromEventsAsync(string templateName, IEnumerable<Event> events)
    {
        CreateExportTemplateCommand template = new() { Name = templateName };
        
        CsvExportTemplate csvTemplate = new()
        {
            Headers = events
                .SelectMany(e => e.Tags ?? new EventTag[] {})
                .Select(t => t.Name)
                .Distinct()
        };
        template.Data = JsonSerializer.Serialize(csvTemplate);
        
        await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/export-templates", template); 
    }
    
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        Delimiter       = ",",
        HasHeaderRecord = true
    };

    private IEnumerable<dynamic> ParseEventsCsv(Stream data)
    {
        using StreamReader reader = new(data);
        using CsvReader    csv    = new(reader, Configuration);

        List<dynamic> events = new();

        while (csv.Read())
            events.Add(csv.GetRecord<dynamic>());

        return events;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.Database.CleanAsync();
}