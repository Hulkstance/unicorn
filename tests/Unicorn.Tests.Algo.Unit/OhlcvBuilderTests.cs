using FluentAssertions;
using Unicorn.Algo.Models;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class OhlcvBuilderTests
{
    [Fact]
    public void Build_ShouldNotThrow_WhenGivenNoAdditionalSetupInformation()
    {
        // Arrange
        var ohlcvBuilder = new OhlcvBuilder();

        // Act
        var action = new Action(() => ohlcvBuilder.Build());

        // Assert
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(15000, 16400, 13500, 16000, 76000)]
    public void Build_ShouldBeConstructed_WhenGivenSetupInformation(decimal open, decimal high, decimal low, decimal close, decimal volume)
    {
        // Arrange
        var ohlcvBuilder = new OhlcvBuilder();
        var expectedOhlcv = new
        {
            Open = open,
            High = high,
            Low = low,
            Close = close,
            Volume = volume
        };

        // Act
        var ohlcv = ohlcvBuilder
            .WithDate(new DateTimeOffset(2022, 5, 1, 8, 6, 32, 545,
                new TimeSpan(1, 0, 0)))
            .WithOpen(open)
            .WithHigh(high)
            .WithLow(low)
            .WithClose(close)
            .WithVolume(volume)
            .Build();

        // Assert
        ohlcv.Should().BeEquivalentTo(expectedOhlcv);
    }

    [Fact]
    public void Build_ShouldThrow_WhenGivenNegativeOpen()
    {
        // Arrange
        const decimal open = -1;
        var ohlcvBuilder = new OhlcvBuilder();

        // Act
        var action = new Action(() => ohlcvBuilder.WithOpen(open));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The * cannot be null. (Parameter '*')");
    }

    [Fact]
    public void Build_ShouldThrow_WhenGivenNegativeHigh()
    {
        // Arrange
        const decimal high = -1;
        var ohlcvBuilder = new OhlcvBuilder();

        // Act
        var action = new Action(() => ohlcvBuilder.WithHigh(high));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The * cannot be null. (Parameter '*')");
    }

    [Fact]
    public void Build_ShouldThrow_WhenGivenNegativeLow()
    {
        // Arrange
        const decimal low = -1;
        var ohlcvBuilder = new OhlcvBuilder();

        // Act
        var action = new Action(() => ohlcvBuilder.WithLow(low));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The * cannot be null. (Parameter '*')");
    }

    [Fact]
    public void Build_ShouldThrow_WhenGivenNegativeClose()
    {
        // Arrange
        const decimal close = -1;
        var ohlcvBuilder = new OhlcvBuilder();

        // Act
        var action = new Action(() => ohlcvBuilder.WithClose(close));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The * cannot be null. (Parameter '*')");
    }

    [Fact]
    public void Build_ShouldThrow_WhenGivenNegativeVolume()
    {
        // Arrange
        const decimal volume = -1;
        var ohlcvBuilder = new OhlcvBuilder();

        // Act
        var action = new Action(() => ohlcvBuilder.WithVolume(volume));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The * cannot be null. (Parameter '*')");
    }
}
