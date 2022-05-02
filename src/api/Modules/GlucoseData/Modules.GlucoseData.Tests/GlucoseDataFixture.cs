using Diagraph.Infrastructure.Tests;
using Diagraph.Modules.GlucoseData.Database;

namespace Diagraph.Modules.GlucoseData.Tests;

public class GlucoseDataFixture : DatabaseModuleFixture<GlucoseDataDbContext>
{
    public GlucoseDataFixture() : base("glucose-data") { }
}