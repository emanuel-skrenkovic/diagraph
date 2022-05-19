using System;
using System.Collections.Generic;
using System.Linq;
using Diagraph.Modules.Events.DataImports.Csv;
using Xunit;

namespace Diagraph.Modules.Events.Tests.Unit;

public class TemplateLanguageParserTests
{
    [Fact]
    public void Assigns_Literal_To_Field()
    {
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new();
        var parser = new TemplateLanguageParser(initialData, data);

        const string date = "2020-01-01";
        const string field = "occurredAtUtc";
        
        var result = parser.ApplyRule($"{field} = \"{date}\"").FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(date, result[field]);
    }

    [Fact]
    public void Appends_Literals()
    {
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new();
        var parser = new TemplateLanguageParser(initialData, data);

        const string part1 = "2020-01-01";
        const string part2 = " ";
        const string part3 = "23:00";
        
        const string field = "occurredAtUtc";
        
        var result = parser
            .ApplyRule($"{field} = \"{part1}\" + \"{part2}\" + \"{part3}\"")
            .FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(part1 + part2 + part3, result[field]); 
    }

    [Fact]
    public void Assigns_Value_From_Data_With_Selector()
    {
        const string date = "2020-01-01";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(initialData, data);
        
        var result = parser.ApplyRule($"{eventField} = @{dataField}").FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(data[dataField], result[eventField]); 
    }
    
    [Fact]
    public void Assigns_Value_From_Data_With_Selector_With_Quoted_Name()
    {
        const string date = "2020-01-01";
        const string dataField = "Field With Space";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(initialData, data);
        
        var result = parser.ApplyRule($"{eventField} = @\"{dataField}\"").FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(data[dataField], result[eventField]); 
    }
    
    [Fact]
    public void Appends_Selector_Data_To_Literal()
    {
        const string date = "23:00";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(initialData, data);

        const string literal = "2020-01-01 ";
        
        var result = parser.ApplyRule($"{eventField} = \"{literal}\" + @{dataField}").FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(literal + data[dataField], result[eventField]); 
    }
    
    [Fact]
    public void Appends_Literal_Data_To_Selector_Data()
    {
        const string date = "23:00";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(initialData, data);

        const string literal = "2020-01-01 ";
        
        var result = parser.ApplyRule($"{eventField} = @{dataField} + \"{literal}\"").FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(data[dataField] + literal, result[eventField]); 
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
        var result = parser.ApplyRule($"{eventField} = @Time.regex(\"\\d*:\\d*\")").FirstOrDefault();
        
        Assert.NotNull(result);
        Assert.Equal(time, result[eventField]);
    }
    
    [Fact]
    public void Assigns_Data_Matching_Selector_Regex_With_Multiple_Matches()
    {
        const string time = "23:00";
        const string eventField = "occurredAtUtc";
         
        Dictionary<string, object> eventData = new();
        Dictionary<string, object> data      = new()
        {
            ["Time"] = $"unrelated text\n{time}\nmore unrelated text {time}"
        }; 
                
        var parser = new TemplateLanguageParser(eventData, data);
        var results = parser.ApplyRule($"{eventField} = @Time.regex(\"\\d*:\\d*\")").ToList();
        
        Assert.Equal(2, results.Count);
        Assert.Contains(results, result => (string)result[eventField] == time);
    }
    
    [Fact]
    public void Append_Data_Matching_Selector_Regex_With_Multiple_Matches()
    {
        const string date = "23:00h";
        const string date2 = "23:30h";
        const string textField = "Field";
        
        const string dateLiteral = "2020-01-01 ";
        const string dateField = "DateTime";
        
        const string eventField = "occurredAtUtc";
         
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data        = new()
        {
            [dateField] = dateLiteral,
            [textField] = $"unrelated text\n{date}\nmore unrelated text {date2}"
        };
                
        var parser = new TemplateLanguageParser(initialData, data);
        var results = parser
            .ApplyRule($"{eventField} = @{dateField} + @{textField}.regex(\"\\d*:\\d*\")")
            .ToList();
        
        Assert.Equal(2, results.Count);
        // TODO: correct assert
        Assert.Contains(results, result => (string)result[eventField] == dateLiteral + "23:00");
        Assert.Contains(results, result => (string)result[eventField] == dateLiteral + "23:30");
    }
    
    [Fact]
    public void Appends_Data_Matching_Selector_Regex_To_Selector()
    {
        const string date = "text 23:00h";
        const string dataField = "Field";
        const string eventField = "occurredAtUtc";
        
        Dictionary<string, object> initialData = new();
        Dictionary<string, object> data      = new() { [dataField] = date };
        var parser = new TemplateLanguageParser(initialData, data);

        const string literal = "2020-01-01 ";
        
        var result = parser
            .ApplyRule($"{eventField} = @{dataField} + \"{literal}\" + @{dataField}.regex(\"\\d*:\\d*\")")
            .FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal($"{data[dataField]}{literal}23:00", result[eventField]); 
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

        var result = parser
            .ApplyRule($"{eventField} = \"2020-01-01\"", new[] { eventField })
            .FirstOrDefault();
        
        Assert.NotNull(result);
        Assert.Contains(result, kv => kv.Key == eventField);
    }
}
