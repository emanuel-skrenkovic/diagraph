using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Diagraph.Modules.Events.Api.DataImports.ImportEvents.Contracts;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class TestFormFile : IFormFile
{
    private string _content;

    public TestFormFile(string content)
    {
        _content = content;
    }

    public Stream OpenReadStream()
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(_content));
    }

    public void CopyTo(Stream target)
    {
        throw new System.NotImplementedException();
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = new())
    {
        Stream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(_content));
        await memoryStream.CopyToAsync(target, cancellationToken);
        await memoryStream.FlushAsync(cancellationToken);
    }

    public string ContentType { get; }
    public string ContentDisposition { get; }
    public IHeaderDictionary Headers { get; }
    public long Length { get; }
    public string Name { get; }
    public string FileName { get; }
}

public class ImportEventsCommandCustomization : ICustomization
{
    public const string TemplateName = "Integration-Test-Template";
    
    public void Customize(IFixture fixture)
    {
        fixture.Customize<IFormFile>
        (
            composer => composer.FromFactory(() => new TestFormFile("TestHeader\nTestValue"))
        );
        fixture.Customize<ImportEventsCommand>(composer => composer.FromFactory
        (
            () => new()
            {
                File         = fixture.Create<IFormFile>(),
                TemplateName = TemplateName
            }));
    }
    

}