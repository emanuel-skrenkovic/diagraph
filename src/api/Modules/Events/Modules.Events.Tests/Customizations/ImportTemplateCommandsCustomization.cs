using System.Text.Json;
using AutoFixture;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Commands;
using Diagraph.Modules.Events.DataImports.Csv;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class ImportTemplateCommandsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<CreateImportTemplateCommand>(composer => composer.FromFactory
        (
            () => new()
            {
                Name = fixture.Create<string>(),
                Data = JsonSerializer.Serialize(fixture.Create<CsvTemplate>())
            }
        ).OmitAutoProperties());
        
        fixture.Customize<UpdateImportTemplateCommand>(composer => composer.FromFactory
        (
            () => new()
            {
                Name = fixture.Create<string>(),
                Data = JsonSerializer.Serialize(fixture.Create<CsvTemplate>())
            }
        ).OmitAutoProperties());
    }
}