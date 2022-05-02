using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Diagraph.Modules.Events.Api.DataImports.Commands;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports.Csv;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class EventsDataImportTests
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
            .Client
            .PostAsync("events/data-import", content);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.ExecuteAsync<EventsDbContext>
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
        await _fixture.Postgres.CleanAsync();
        
        // Arrange
        const string templateName = "Integration-Test-Template";
        await InsertTemplate(templateName);
        HttpContent content = EventDataImportContent(templateName);
        
        // Act
        HttpResponseMessage response = await _fixture
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

        await _fixture.ExecuteAsync<EventsDbContext>
        (
            async context =>
            {
                (await context.Events.AnyAsync()).Should().BeFalse();
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
                        Tags = new [] { "Tag1", "Tag2" }
                    }
                }
            })
        };
        
        await _fixture
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
}