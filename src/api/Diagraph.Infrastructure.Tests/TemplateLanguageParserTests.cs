using System.Collections.Generic;
using Diagraph.Infrastructure.Parsing.Language;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class TemplateLanguageParserTests
{
    [Fact]
    public void Test1()
    {
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new()
        {
            ["Datum"] = "2022-01-01",
            ["Vrijeme 2"] = "test tekst teskt teskt\n23:00h"
        }; 
                
        var parser = new TemplateLanguageParser(eventData, data);
        parser.ApplyRule
        (
            "occurredAtUtc = @Datum + \" \" + @\"Vrijeme 2\".regex(\"\\d*:\\d*\")",
            new [] { "asdf" }
        );
    }
}
