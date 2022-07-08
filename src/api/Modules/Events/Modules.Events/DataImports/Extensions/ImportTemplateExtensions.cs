namespace Diagraph.Modules.Events.DataImports.Extensions;

public static class ImportTemplateExtensions
{
    public static string SourceName(this ImportTemplate template)
        => $"template_{template.Name}_{template.Id}";
}