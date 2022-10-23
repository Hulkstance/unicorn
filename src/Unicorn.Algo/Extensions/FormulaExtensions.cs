namespace Unicorn.Algo.Extensions;

public static class FormulaExtensions
{
    public static double Mean(this List<double> values)
    {
        return values.Count == 0 ? 0 : values.Mean(0, values.Count);
    }

    public static double Mean(this List<double> values, int start, int end)
    {
        double sum = 0;

        for (var i = start; i < end; i++)
        {
            sum += values[i];
        }

        return sum / (end - start);
    }

    public static double Variance(this List<double> values)
    {
        return values.Variance(values.Mean(), 0, values.Count);
    }

    public static double Variance(this List<double> values, double mean)
    {
        return values.Variance(mean, 0, values.Count);
    }

    public static double Variance(this List<double> values, double mean, int start, int end)
    {
        double variance = 0;

        for (var i = start; i < end; i++)
        {
            variance += Math.Pow(values[i] - mean, 2);
        }

        var n = end - start;

        if (start > 0)
        {
            n -= 1;
        }

        return variance / (n - 1);
    }

    public static double StandardDeviation(this List<double> values)
    {
        return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
    }

    public static double StandardDeviation(this List<double> values, int start, int end)
    {
        var mean = values.Mean(start, end);
        var variance = values.Variance(mean, start, end);

        return Math.Sqrt(variance);
    }

    public static double PopulationVariance(this List<double> values)
    {
        return values.PopulationVariance(values.Mean(), 0, values.Count);
    }

    public static double PopulationVariance(this List<double> values, double mean)
    {
        return values.PopulationVariance(mean, 0, values.Count);
    }

    public static double PopulationVariance(this List<double> values, double mean, int start, int end)
    {
        double variance = 0;

        for (var i = start; i < end; i++)
        {
            variance += Math.Pow(values[i] - mean, 2);
        }

        var n = end - start;

        if (start > 0)
        {
            n -= 1;
        }

        return variance / n;
    }

    public static double PopulationStandardDeviation(this List<double> values)
    {
        return values.Count == 0 ? 0 : values.PopulationStandardDeviation(0, values.Count);
    }

    public static double PopulationStandardDeviation(this List<double> values, int start, int end)
    {
        var mean = values.Mean(start, end);
        var variance = values.PopulationVariance(mean, start, end);

        return Math.Sqrt(variance);
    }
}
