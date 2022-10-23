using FluentAssertions;
using Unicorn.Algo.Indicators;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class SimpleMovingAverageTests
{
    [Fact]
    public void ComputeNextValue_ShouldReturnExpectedValues_WhenGivenPrices()
    {
        // Arrange
        const int period = 4;
        var prices = new decimal[] { 10, 12, 9, 10, 15, 13, 18, 18, 20, 24 };
        var expected = new[] { 0, 0, 0, 10.25m, 11.5m, 11.75m, 14, 16, 17.25m, 20 };

        var sma = new SimpleMovingAverage(period);

        // Act
        var actual = prices
            .Select(x => sma.ComputeNextValue(x))
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

        var sma = new SimpleMovingAverage(period);

        // Act
        var actual = new List<bool>();
        foreach (var price in prices)
        {
            sma.ComputeNextValue(price);
            actual.Add(sma.IsReady);
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
        var expected = new[] { 0, 0, 0, 0, 0, 0, 14, 16, 17.25m, 20 };

        var sma = new SimpleMovingAverage(period);

        // Act
        var actual = new List<decimal>();
        for (var i = 0; i < prices.Length; i++)
        {
            if (i == 3)
            {
                sma.Reset();
            }

            actual.Add(sma.ComputeNextValue(prices[i]));
        }

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
