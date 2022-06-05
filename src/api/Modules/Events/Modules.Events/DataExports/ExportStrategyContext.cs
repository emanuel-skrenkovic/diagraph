namespace Diagraph.Modules.Events.DataExports;

public enum DataExportStrategy
{
    Individual,
    Merging
}

// TODO: This is so bad! #horribleways
public class ExportStrategyContext
{
    private readonly IDictionary<DataExportStrategy, Type> _strategyKinds = new Dictionary<DataExportStrategy, Type>()
    {
        [DataExportStrategy.Individual] = typeof(IndividualDataExportStrategy),
        [DataExportStrategy.Merging]    = typeof(MergingDataExportStrategy)
    };
    
    private readonly IDictionary<Type, IDataExportStrategy> _strategies;

    public ExportStrategyContext(IEnumerable<IDataExportStrategy> strategies)
    {
        _strategies = strategies?.ToDictionary(s => s.GetType(), s => s);
    }

    public IDataExportStrategy GetStrategy(DataExportStrategy strategy) 
        => _strategies[_strategyKinds[strategy]];
}