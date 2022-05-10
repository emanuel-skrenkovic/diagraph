using AutoFixture;
using Diagraph.Modules.Events.DataImports.Csv;

namespace Diagraph.Modules.Events.Tests.Customizations;

public class CsvTemplateCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<CsvTemplate>(composer => composer.FromFactory
        (
            () => new () 
            {
                HeaderMappings = new []
                {
                    new HeaderMappings
                    {
                        Header = "TestHeader",
                        Rules = new []
                        {
                            new Rule
                            {
                                Expression = "occurredAtUtc = \"2020-01-01\""
                            }
                        },
                        Tags = new [] { new EventTag { Name = "Tag1" } , new EventTag { Name = "Tag2" } }
                    }
                }
            }
        ));
    }
}