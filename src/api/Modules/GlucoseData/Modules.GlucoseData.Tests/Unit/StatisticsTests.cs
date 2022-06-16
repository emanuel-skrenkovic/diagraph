using System.Collections.Generic;
using Diagraph.Infrastructure.Tests.AutoFixture;
using FluentAssertions;
using Xunit;

namespace Diagraph.Modules.GlucoseData.Tests.Unit;

public class StatisticsTests
{
    [Fact]
    public void Calculates_Statistics_From_No_Measurements()
    {
        GlucoseStatistics glucoseStatistics = GlucoseStatistics.Calculate(new GlucoseMeasurement[] {});
        glucoseStatistics.Should().NotBeNull();
    }

    [Theory, CustomizedAutoData]
    public void Calculates_Statistics(IEnumerable<GlucoseMeasurement> measurements)
    {
        GlucoseStatistics glucoseStatistics = GlucoseStatistics.Calculate(measurements);
        glucoseStatistics.Should().NotBeNull();
        glucoseStatistics.Mean.Should().NotBe(0);
        glucoseStatistics.Median.Should().NotBe(0);
        glucoseStatistics.GlucoseManagementIndicator.Should().NotBe(0);
        glucoseStatistics.GlucoseManagementIndicatorPercentage.Should().NotBe(0); 
    }
}