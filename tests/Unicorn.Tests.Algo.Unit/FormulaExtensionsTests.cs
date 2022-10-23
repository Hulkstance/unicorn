﻿using FluentAssertions;
using Unicorn.Algo.Extensions;
using Xunit;

namespace Unicorn.Tests.Algo.Unit;

public class FormulaExtensionsTests
{
    [Theory]
    [InlineData(new double[] { 40, 30, 20 }, 30)]
    [InlineData(new double[] { 40, 30, 20, 50, 100, 150, 200, 250, 10, 25 }, 87.5)]
    public void Mean_ShouldBeCorrect_WhenGivenListOfValues(double[] input, double expectedMean)
    {
        // Arrange
        var source = input.ToList();

        // Act
        var mean = source.Mean();

        // Assert
        mean.Should().Be(expectedMean);
    }

    [Theory]
    [InlineData(new double[] { 40, 30, 20 }, 100, 66.66666666666667)]
    [InlineData(new double[] { 40, 30, 20, 50, 100, 150, 200, 250, 10, 25 }, 7173.611111111111, 6456.25)]
    public void Variance_ShouldBeCorrect_WhenGivenListOfValues(
        double[] input,
        double expectedVariance,
        double expectedPopulationVariance)
    {
        // Arrange
        var source = input.ToList();

        // Act
        var variance = source.Variance();
        var populationVariance = source.PopulationVariance();

        // Assert
        variance.Should().Be(expectedVariance);
        populationVariance.Should().Be(expectedPopulationVariance);
    }

    [Theory]
    [InlineData(new double[] { 40, 30, 20 }, 30, 100, 66.66666666666667)]
    [InlineData(new double[] { 40, 30, 20, 50, 100, 150, 200, 250, 10, 25 }, 87.5, 7173.611111111111, 6456.25)]
    public void Variance_ShouldBeCorrect_WhenGivenMean(
        double[] input,
        double mean,
        double expectedVariance,
        double expectedPopulationVariance)
    {
        // Arrange
        var source = input.ToList();

        // Act
        var variance = source.Variance(mean);
        var populationVariance = source.PopulationVariance(mean);

        // Assert
        variance.Should().Be(expectedVariance);
        populationVariance.Should().Be(expectedPopulationVariance);
    }

    [Theory]
    [InlineData(new double[] { 40, 30, 20 }, 10, 8.16496580927726)]
    [InlineData(new double[] { 40, 30, 20, 50, 100, 150, 200, 250, 10, 25 }, 84.69717298181274, 80.35079339994098)]
    public void StandardDeviation_ShouldBeCorrect_WhenGivenListOfValues(
        double[] input,
        double expectedStdDev,
        double expectedPopulationStdDev)
    {
        // Arrange
        var source = input.ToList();

        // Act
        var stdDev = source.StandardDeviation();
        var populationStdDev = source.PopulationStandardDeviation();

        // Assert
        stdDev.Should().Be(expectedStdDev);
        populationStdDev.Should().Be(expectedPopulationStdDev);
    }
}
