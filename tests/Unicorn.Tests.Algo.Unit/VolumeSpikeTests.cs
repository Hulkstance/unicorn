using FluentAssertions;
using Unicorn.Algo.Indicators;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class VolumeSpikeTests
{
    [Fact]
    public void ComputeNextValue_ShouldReturnExpectedValues_WhenGivenVolumes()
    {
        // Arrange
        const int period = 20;
        var volumes = new[]
        {
            0.18600000m, 0.37000000m, 3.07000000m, 0.06600000m, 3.51700000m, 0.27700000m, 0.03700000m, 0.36300000m,
            0.73800000m, 0.20900000m, 0.06100000m, 0.09000000m, 0.16900000m, 0.05000000m, 0.04000000m, 0.06100000m,
            0.12100000m, 0.29200000m, 0.25000000m, 0.18400000m, 0.17100000m, 0.04200000m, 0.03700000m, 0.31600000m,
            0.10000000m, 1.83500000m, 2.16500000m, 0.03900000m, 0.08100000m, 1.75800000m, 0.20900000m, 2.09000000m,
            0.35700000m, 0.56400000m, 1.00000000m
        };
        var expected = new bool?[]
        {
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, false, false, false, false, false, false, true, true, false, false, true, false, true,
            false, false, false
        };

        var volumeSpike = new VolumeSpike(period, 2);

        // Act
        var actual = volumes
            .Select(x => volumeSpike.ComputeNextValue(x))
            .ToList();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
