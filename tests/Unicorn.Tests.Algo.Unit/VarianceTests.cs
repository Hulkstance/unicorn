using FluentAssertions;
using Unicorn.Algo.Indicators;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class VarianceTests
{
    [Fact]
    public void ComputeNextValue_ShouldReturnExpectedValues_WhenGivenPrices()
    {
        // Arrange
        const int period = 4;
        var prices = new decimal[] { 10, 12, 9, 10, 15, 13, 18, 18, 20, 24 };
        var expected = new[] { 0, 0, 0, 1.1875m, 5.25m, 5.6875m, 8.5m, 4.5m, 6.6875m, 6 };

        var variance = new Variance(period);

        // Act
        var actual = prices
            .Select(x => variance.ComputeNextValue(x))
            .ToList();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void IsReady_ShouldReturnExpectedValues_WhenPeriodIsReached()
    {
        // Arrange
        const int period = 4;
        var prices = new decimal[] { 10, 12, 9, 10, 15, 13, 18, 18, 20, 24 };
        var expected = new[] { false, false, false, true, true, true, true, true, true, true };

        var variance = new Variance(period);

        // Act
        var actual = new List<bool>();
        foreach (var price in prices)
        {
            variance.ComputeNextValue(price);
            actual.Add(variance.IsReady);
        }

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Reset_ShouldResetQueue()
    {
        // Arrange
        const int period = 4;
        var prices = new decimal[] { 10, 12, 9, 10, 15, 13, 18, 18, 20, 24 };
        var expected = new[] { 0, 0, 0, 0, 0, 0, 8.5m, 4.5m, 6.6875m, 6 };

        var variance = new Variance(period);

        // Act
        var actual = new List<decimal>();
        for (var i = 0; i < prices.Length; i++)
        {
            if (i == 3)
            {
                variance.Reset();
            }

            actual.Add(variance.ComputeNextValue(prices[i]));
        }

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
