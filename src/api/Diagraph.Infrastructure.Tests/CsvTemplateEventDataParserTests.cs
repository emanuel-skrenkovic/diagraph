using System.IO;
using Diagraph.Infrastructure.Parsing;
using Diagraph.Infrastructure.Parsing.Templates;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class CsvTemplateEventDataParserTests
{
    [Fact]
    public void Parse()
    {
        var template = new CsvTemplate
        {
            HeaderMappings = new []
            {
                new HeaderMappings { Header = "Datum", Tags = new [] { "Datum" } },
                new HeaderMappings { Header = "Dan",   Tags = new [] { "Dan" } }
            }
        };
        var parser = new EventCsvTemplateDataParser();

        const string path = "/home/emanuel/Downloads/Dijabetes-Sheet1.csv";
        parser.Parse(File.ReadAllText(path), template);
    }
}