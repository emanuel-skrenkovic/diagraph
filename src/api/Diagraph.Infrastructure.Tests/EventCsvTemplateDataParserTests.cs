using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing;
using Diagraph.Infrastructure.Parsing.Templates;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class EventCsvTemplateDataParserTests
{
    [Fact]
    public void Test1()
    {
        IMapper mapper = new MapperConfiguration(_ => {}).CreateMapper();
        var parser = new EventCsvTemplateDataParser(mapper);

        const string path = "/home/emanuel/Downloads/Dijabetes-Sheet1.csv";
        IEnumerable<Event> events = parser.Parse
        (
            File.ReadAllText(path),
            new()
            {
                HeaderMappings = new []
                {
                    new HeaderMappings
                    {
                        Header = "Datum",
                        Tags = new [] { "Date" }
                    },
                    new HeaderMappings
                    {
                        Header = "Međuobrok 1",
                        Tags = new [] { "Meal 1" },
                        Rules = new []
                        {
                            new Rule
                            {
                                RegEx = "\\d*:\\d*",
                                FieldName = "OccurredAtUtc"
                            }
                        }
                    }
                }
            }
        );
    }
}