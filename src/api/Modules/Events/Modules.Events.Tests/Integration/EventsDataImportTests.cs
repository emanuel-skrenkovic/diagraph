using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Diagraph.Modules.Events.Api.DataImports.ImportEvents;
using Diagraph.Modules.Events.Api.DataImports.ImportEvents.Contracts;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports.Csv;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventsDataImportTests : IAsyncLifetime
{
    private readonly EventsFixture _fixture;

    public EventsDataImportTests(EventsFixture fixture)
        => _fixture = fixture;
    
    [Fact]
    public async Task Imports_Events_Using_Template()
    {
        // Arrange
        const string templateName = "Integration-Test-Template";
        await InsertTemplate(templateName);
        HttpContent content = EventDataImportContent(templateName);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsync("events/data-import", content);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.Module.ExecuteAsync<EventsDbContext>
        (
            async context =>
            {
                (await context.Events.AnyAsync()).Should().BeTrue();
            }
        );
    }

    [Fact]
    public async Task Dry_Run_Does_Not_Create_Events()
    {
        // Arrange
        const string templateName = "Integration-Test-Template";
        await InsertTemplate(templateName);
        HttpContent content = EventDataImportContent(templateName);
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsync("events/data-import/dry-run", content);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        IEnumerable<Event> events = JsonSerializer.Deserialize<IEnumerable<Event>>
        (
            await response.Content.ReadAsStringAsync()
        );
        events.Should().NotBeEmpty();

        await _fixture.Module.ExecuteAsync<EventsDbContext>
        (
            async context =>
            {
                (await context.Events.AnyAsync()).Should().BeFalse();
            }
        );
    }
    
    [Fact]
    public async Task Imports_Google_Fit_Events()
    {
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsync("events/data-import/google/fitness", null);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonSerializer.Deserialize<ImportGoogleFitnessActivitiesResult>
        (
            await response.Content.ReadAsByteArrayAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThan(0);

        await _fixture.Module.ExecuteAsync<EventsDbContext>
        (
            async context =>
            {
                List<Event> events = await context
                    .Events
                    .Where(e => e.Source == ImportGoogleFitnessActivitiesEndpoint.GoogleFitnessSource)
                    .ToListAsync();
                
                events.Count.Should().BeGreaterThan(0);
                events.Count.Should().Be(result.Count);
            }
        );
    }
    
    [Fact]
    public async Task Does_Not_Re_Import_Identical_Google_Fit_Events()
    {
        // Arrange
        HttpResponseMessage initialResponse = await _fixture
            .Module
            .Client
            .PostAsync("events/data-import/google/fitness", null);
        
        var initialResult = JsonSerializer.Deserialize<ImportGoogleFitnessActivitiesResult>
        (
            await initialResponse.Content.ReadAsByteArrayAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        int count = initialResult!.Count;
        
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsync("events/data-import/google/fitness", null);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonSerializer.Deserialize<ImportGoogleFitnessActivitiesResult>
        (
            await response.Content.ReadAsByteArrayAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        result.Should().NotBeNull();
        result!.Count.Should().Be(0);

        await _fixture.Module.ExecuteAsync<EventsDbContext>
        (
            async context =>
            {
                List<Event> events = await context
                    .Events
                    .Where(e => e.Source == ImportGoogleFitnessActivitiesEndpoint.GoogleFitnessSource)
                    .ToListAsync();
                
                events.Count.Should().BeGreaterThan(0);
                events.Count.Should().Be(initialResult.Count);

                events.DistinctBy(e => e.Discriminator).Count().Should().Be(count);
            }
        );
    }

    private async Task InsertTemplate(string templateName)
    {
        CreateImportTemplateCommand createTemplate = new()
        {
            Name = templateName,
            Data = JsonSerializer.Serialize(new CsvTemplate
            {
                HeaderMappings = new []
                {
                    new HeaderMappings
                    {
                        Header = "header",
                        Rules = new []
                        {
                            new Rule
                            {
                                Expression = "occurredAtUtc = \"2020-01-01\""
                            }
                        },
                        Tags = new [] { new EventTag { Name = "Tag1" }, new EventTag { Name = "Tag2" } }
                    }
                }
            })
        };
        
        await _fixture
            .Module
            .Client
            .PostAsJsonAsync("events/import-templates", createTemplate); 
    }

    private HttpContent EventDataImportContent(string templateName)
    {
        MultipartFormDataContent content = new();
        content.Add
        (
            new StreamContent
            (
                new MemoryStream(Encoding.UTF8.GetBytes("header,header1\nTestValue,TestValue2"))
            ),
            name: "file",
            fileName: "file"
        );
        content.Add(new StringContent(templateName), name: "templateName");  
        
        return content;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.Database.CleanAsync();
}