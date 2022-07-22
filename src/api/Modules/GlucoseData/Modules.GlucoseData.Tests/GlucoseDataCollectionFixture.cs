using Xunit;

namespace Diagraph.Modules.GlucoseData.Tests;

[CollectionDefinition(nameof(GlucoseDataCollectionFixture))]
public class GlucoseDataCollectionFixture : IClassFixture<GlucoseDataFixture> { }