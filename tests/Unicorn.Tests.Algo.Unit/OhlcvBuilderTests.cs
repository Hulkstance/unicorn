using FluentAssertions;
using Unicorn.Algo.Models;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class OhlcvBuilderTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(15000, 16400, 13500, 16000, 76000)]
    public void Build_ShouldBeAsExpected_WhenParametersArePassed(
        decimal open, decimal high, decimal low, decimal close, decimal volume)
    {
        // Arrange
        var sut = new OhlcvBuilder();
        var expectedOhlcv = new
        {
            Open = open,
            High = high,
            Low = low,
            Close = close,
            Volume = volume
        };

        // Act
        var ohlcv = sut
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
    public void Build_ShouldNotThrow_WhenNoParametersArePassed()
    {
        // Arrange
        var sut = new OhlcvBuilder();

        // Act
        var action = new Action(() => sut.Build());

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void WithOpen_ShouldThrow_WhenOpenIsNegative()
    {
        // Arrange
        const decimal open = -1;
        var sut = new OhlcvBuilder();

        // Act
        var action = new Action(() => sut.WithOpen(open));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Open cannot be negative *");
    }

    [Fact]
    public void WithHigh_ShouldThrow_WhenHighIsNegative()
    {
        // Arrange
        const decimal high = -1;
        var sut = new OhlcvBuilder();

        // Act
        var action = new Action(() => sut.WithHigh(high));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("High cannot be negative *");
    }

    [Fact]
    public void WithLow_ShouldThrow_WhenLowIsNegative()
    {
        // Arrange
        const decimal low = -1;
        var sut = new OhlcvBuilder();

        // Act
        var action = new Action(() => sut.WithLow(low));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Low cannot be negative *");
    }

    [Fact]
    public void WithClose_ShouldThrow_WhenCloseIsNegative()
    {
        // Arrange
        const decimal close = -1;
        var sut = new OhlcvBuilder();

        // Act
        var action = new Action(() => sut.WithClose(close));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Close cannot be negative *");
    }

    [Fact]
    public void WithVolume_ShouldThrow_WhenVolumeIsNegative()
    {
        // Arrange
        const decimal volume = -1;
        var sut = new OhlcvBuilder();

        // Act
        var action = new Action(() => sut.WithVolume(volume));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Volume cannot be negative *");
    }
}
