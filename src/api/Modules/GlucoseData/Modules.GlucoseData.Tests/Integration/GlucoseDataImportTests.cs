using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.GlucoseData.Database;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diagraph.Modules.GlucoseData.Tests.Integration;

[Collection(nameof(GlucoseDataCollectionFixture))]
public class GlucoseDataImportTests : IAsyncLifetime
{
    private readonly GlucoseDataFixture _fixture;

    public GlucoseDataImportTests(GlucoseDataFixture fixture)
        => _fixture = fixture;
    
    [Theory, CustomizedAutoData]
    public async Task Imports_Data_From_LibreView_Format_Csv(IFormFile file)
    {
        // Arrange
        HttpContent content = await ImportContent(file);
        
        // Act
        HttpResponseMessage response = await _fixture.Client.PostAsync("/data/imports", content);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await _fixture.ExecuteAsync<GlucoseDataDbContext>(async context =>
        {
            (await context.Imports.CountAsync()).Should().Be(1);
        });
    }

    [Theory, CustomizedAutoData]
    public async Task Imports_Data_Only_Once_From_Same_File(IFormFile file1, IFormFile file2)
    {
        // Arrange
        HttpContent content1 = await ImportContent(file1);
        HttpContent content2 = await ImportContent(file2);

        // Act
        HttpResponseMessage response1 = await _fixture.Client.PostAsync("/data/imports", content1);
        HttpResponseMessage response2 = await _fixture.Client.PostAsync("/data/imports", content2);
        
        // Assert
        response1.IsSuccessStatusCode.Should().BeTrue();
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        
        response2.IsSuccessStatusCode.Should().BeTrue();
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.ExecuteAsync<GlucoseDataDbContext>(async context =>
        {
            (await context.Imports.CountAsync()).Should().Be(1);
        }); 
    }

    [Fact]
    public async Task Returns_400_If_No_File_Sent()
    {
        // Act
        HttpResponseMessage response = await _fixture.Client.PostAsync("/data/imports", null); 
        
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<HttpContent> ImportContent(IFormFile file)
    {
        MultipartFormDataContent content = new();
        content.Add
        (
            new StreamContent
            (
                new MemoryStream(Encoding.UTF8.GetBytes(await file.ReadAsync()))
            ),
            name: "file",
            fileName: "file"
        );

        return content;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.Postgres.CleanAsync();
}