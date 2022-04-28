using AutoMapper;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;

namespace Diagraph.Modules.Events.DataImports.Templates;

public class TemplateRunnerFactory
{
    private readonly IMapper _mapper;

    public TemplateRunnerFactory(IMapper mapper)
        => _mapper = mapper;
     
    public ITemplateRunner FromTemplate<T>(T template)
    {
        return template switch
        {
            CsvTemplate csvTemplate => new CsvTemplateRunner(_mapper, csvTemplate),
            _                       => throw new ArgumentOutOfRangeException(nameof(template))
        };
    }
}