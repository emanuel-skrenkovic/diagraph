using Xunit;

namespace Diagraph.Modules.Identity.Tests;

[CollectionDefinition(nameof(IdentityFixtureCollection))]
public class IdentityFixtureCollection: ICollectionFixture<IdentityFixture> { }