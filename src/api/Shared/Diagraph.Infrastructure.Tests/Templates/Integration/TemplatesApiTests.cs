using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Diagraph.Api.Templates.Models;
using Diagraph.Infrastructure.Parsing.Templates;
using Diagraph.Infrastructure.Tests.Fixture;
using FluentAssertions;
using Xunit;

namespace Diagraph.Infrastructure.Tests.Templates.Integration;

public class TemplatesApiTests : IClassFixture<DiagraphFixture>
{
    private readonly DiagraphFixture _fixture;

    public TemplatesApiTests(DiagraphFixture fixture) => _fixture = fixture;
    
    [Theory, AutoData]
    public async Task Creates_Template(ImportTemplateRequest request)
    {
        // Act
        await _fixture.PostAsJsonAsync("/import-templates", request);
        
        // Assert
        ImportTemplate template  = await _fixture.TemplatesHelper.Get(0);

        template.Should().NotBeNull();
        template.Name.Should().BeEquivalentTo(request.Name);
    }
}