using System;
using System.Collections.Generic;
using Diagraph.Modules.Events.DataImports.Csv;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Unit;

public class TemplateLanguageParserTests
{
    [Fact]
    public void Assigns_Literal_To_Field()
    {
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new();
        var parser = new TemplateLanguageParser(eventData, data);

        const string date = "2020-01-01";
        const string field = "occurredAtUtc";
        
        parser.ApplyRule($"{field} = \"{date}\"");

        Assert.Equal(date, eventData[field]);
    }

    [Fact]
    public void Appends_Literals()
    {
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new();
        var parser = new TemplateLanguageParser(eventData, data);

        const string part1 = "2020-01-01";
        const string part2 = " ";
        const string part3 = "23:00";
        
        const string field = "occurredAtUtc";
        
        parser.ApplyRule($"{field} = \"{part1}\" + \"{part2}\" + \"{part3}\"");

        Assert.Equal(part1 + part2 + part3, eventData[field]); 
    }

    [Fact]
    public void Assigns_Value_From_Data_With_Selector()
    {
        const string date = "2020-01-01";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(eventData, data);
        
        parser.ApplyRule($"{eventField} = @{dataField}");

        Assert.Equal(data[dataField], eventData[eventField]); 
    }
    
    [Fact]
    public void Assigns_Value_From_Data_With_Selector_With_Quoted_Name()
    {
        const string date = "2020-01-01";
        const string dataField = "Field With Space";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(eventData, data);
        
        parser.ApplyRule($"{eventField} = @\"{dataField}\"");

        Assert.Equal(data[dataField], eventData[eventField]); 
    }
    
    [Fact]
    public void Appends_Selector_Data_To_Literal()
    {
        const string date = "23:00";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(eventData, data);

        const string literal = "2020-01-01 ";
        
        parser.ApplyRule($"{eventField} = \"{literal}\" + @{dataField}");

        Assert.Equal(literal + data[dataField], eventData[eventField]); 
    }
    
    [Fact]
    public void Appends_Literal_Data_To_Selector_Data()
    {
        const string date = "23:00";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(eventData, data);

        const string literal = "2020-01-01 ";
        
        parser.ApplyRule($"{eventField} = @{dataField} + \"{literal}\"");

        Assert.Equal(data[dataField] + literal, eventData[eventField]); 
    }

    [Fact]
    public void Assigns_Data_Matching_Selector_Regex()
    {
        const string time = "23:00";
        const string eventField = "occurredAtUtc";
         
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new()
        {
            ["Time"] = $"unrelated text\n{time}"
        }; 
                
        var parser = new TemplateLanguageParser(eventData, data);
        parser.ApplyRule($"{eventField} = @Time.regex(\"\\d*:\\d*\")"); 
        
        Assert.Equal(time, eventData[eventField]);
    }

    [Fact]
    public void Throws_Exception_When_Mapping_UnAllowed_Field()
    {
        const string eventField = "not-allowed";
         
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new();
                
        var parser = new TemplateLanguageParser(eventData, data);
        
        Assert.Throws<Exception>
        (
            () => parser.ApplyRule($"{eventField} = \"2020-01-01\"", new [] { "allowed" })
        );
    }
    
    [Fact]
    public void Does_Not_Throw_Exception_When_Mapping_Allowed_Field()
    {
        const string eventField = "allowed";
         
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new();
                
        var parser = new TemplateLanguageParser(eventData, data);

        parser.ApplyRule($"{eventField} = \"2020-01-01\"", new[] { eventField });
        
        Assert.Contains(eventData, kv => kv.Key == eventField);
    }
}
