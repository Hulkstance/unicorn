using FluentAssertions;
using Unicorn.Algo.Indicators;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class StandardDeviationTests
{
    [Fact]
    public void ComputeNextValue_ShouldReturnExpectedValues_WhenGivenPrices()
    {
        // Arrange
        const int period = 4;
        var prices = new decimal[] { 10, 12, 9, 10, 15, 13, 18, 18, 20, 24 };
        var expected = new[]
        {
            0, 0, 0, 1.08972473588517, 2.29128784747792, 2.38484800354236,
            2.91547594742265, 2.12132034355964, 2.58602010819715, 2.44948974278318
        };

        var sut = new StandardDeviation(period);

        // Act
        var actual = prices
            .Select(x => sut.ComputeNextValue(x))
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

        var sut = new StandardDeviation(period);

        // Act
        var actual = new List<bool>();
        foreach (var price in prices)
        {
            sut.ComputeNextValue(price);
            actual.Add(sut.IsReady);
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
        var expected = new[]
        {
            0, 0, 0, 0, 0, 0, 2.91547594742265, 2.12132034355964, 2.58602010819715, 2.44948974278318
        };

        var sut = new StandardDeviation(period);

        // Act
        var actual = new List<decimal>();
        for (var i = 0; i < prices.Length; i++)
        {
            if (i == 3)
            {
                sut.Reset();
            }

            actual.Add(sut.ComputeNextValue(prices[i]));
        }

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
