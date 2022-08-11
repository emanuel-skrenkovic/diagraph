using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Infrastructure.Tests.Extensions;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class ExportTemplatesTests
{
    private readonly EventsFixture _fixture;

    public ExportTemplatesTests(EventsFixture fixture) => _fixture = fixture;

    [Theory, CustomizedAutoData]
    public async Task Creates_ExportTemplate(CreateExportTemplateCommand createExportTemplate)
    {
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/export-templates", createExportTemplate);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        int id = response.CreatedId<int>();
        
        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            var createdTemplate = await context.FindAsync<ExportTemplate>(id);
            
            createdTemplate.Should().NotBeNull();
            createdTemplate!.Name.Should().Be(createExportTemplate.Name);
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task Updates_ExportTemplate
    (
        CreateExportTemplateCommand create, 
        UpdateExportTemplateCommand update
    )
    {
        // Arrange
        HttpResponseMessage createResponse = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/export-templates", create);
        
        int id = createResponse.CreatedId<int>();
        
        // Act
        HttpResponseMessage updateResponse = await _fixture
            .Module
            .Client
            .PutAsJsonAsync($"/events/export-templates/{id}", update);

        updateResponse.IsSuccessStatusCode.Should().BeTrue();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            var updatedTemplate = await context.FindAsync<ExportTemplate>(id);

            updatedTemplate.Should().NotBeNull();
            updatedTemplate!.Name.Should().Be(update.Name);
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task Deletes_ExportTemplate
    (
        CreateExportTemplateCommand create
    )
    {
        // Arrange
        HttpResponseMessage createResponse = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/export-templates", create);
        
        int id = createResponse.CreatedId<int>();
        
        // Act
        HttpResponseMessage deleteResponse = await _fixture
            .Module
            .Client
            .DeleteAsync($"/events/export-templates/{id}");

        deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            var deletedTemplate = await context
                .ExportTemplates
                .SingleOrDefaultAsync(t => t.Id == id);
            deletedTemplate.Should().BeNull();
        });
    }
}