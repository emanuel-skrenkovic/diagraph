using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Infrastructure.Tests.Extensions;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using FluentAssertions;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Integration;

[Collection(nameof(EventsCollectionFixture))]
public class ImportTemplatesTests
{
    private readonly EventsFixture _fixture;

    public ImportTemplatesTests(EventsFixture fixture)
        => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Creates_ImportTemplate(CreateImportTemplateCommand createImportTemplate)
    {
        // Act
        HttpResponseMessage response = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/import-templates", createImportTemplate);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        int id = response.CreatedId<int>();

        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            ImportTemplate createdTemplate = await context.FindAsync<ImportTemplate>(id);
            
            createdTemplate.Should().NotBeNull();
            createdTemplate!.Name.Should().Be(createImportTemplate.Name);
        });
    }

    [Theory, CustomizedAutoData]
    public async Task Updates_ImportTemplate
    (
        CreateImportTemplateCommand create, 
        UpdateImportTemplateCommand update
    )
    {
        // Arrange
        HttpResponseMessage createResponse = await _fixture
            .Module
            .Client
            .PostAsJsonAsync("/events/import-templates", create);
        
        int id = createResponse.CreatedId<int>();
        
        // Act
        HttpResponseMessage updateResponse = await _fixture
            .Module
            .Client
            .PutAsJsonAsync($"/events/import-templates/{id}", update);

        updateResponse.IsSuccessStatusCode.Should().BeTrue();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.Module.ExecuteAsync<EventsDbContext>(async context =>
        {
            ImportTemplate updatedTemplate = await context.FindAsync<ImportTemplate>(id);

            updatedTemplate.Should().NotBeNull();
            updatedTemplate!.Name.Should().Be(update.Name);
        });
    }
}