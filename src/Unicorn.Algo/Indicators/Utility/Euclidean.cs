namespace Unicorn.Algo.Indicators.Utility;

public static class Euclidean
{
    public static double Mod(double numerator, double denominator)
    {
        var quotient = Math.Floor(numerator / denominator);
        return numerator - (quotient * denominator);
    }

    public static double Wrap(double value, double minimum, double range)
    {
        var transform = value - minimum;
        var remainder = Mod(transform, range);
        return remainder + minimum;
    }

    public static int GreatestCommonDenominator(this IEnumerable<int> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(GreatestCommonDenominator);
    }

    public static int GreatestCommonDenominator(params int[] source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(GreatestCommonDenominator);
    }

    public static int GreatestCommonDenominator(int a, int b)
    {
        return b != 0 ? GreatestCommonDenominator(b, a % b) : a;
    }

    public static long GreatestCommonDenominator(this IEnumerable<long> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(GreatestCommonDenominator);
    }

    public static long GreatestCommonDenominator(params long[] source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(GreatestCommonDenominator);
    }

    public static long GreatestCommonDenominator(long a, long b)
    {
        return b != 0 ? GreatestCommonDenominator(b, a % b) : a;
    }

    public static int LeastCommonMultiple(this IEnumerable<int> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(LeastCommonMultiple);
    }

    public static int LeastCommonMultiple(params int[] source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(LeastCommonMultiple);
    }

    public static int LeastCommonMultiple(int a, int b)
    {
        return a * (b / GreatestCommonDenominator(a, b));
    }

    public static long LeastCommonMultiple(this IEnumerable<long> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(LeastCommonMultiple);
    }

    public static long LeastCommonMultiple(params long[] source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Aggregate(LeastCommonMultiple);
    }

    public static long LeastCommonMultiple(long a, long b)
    {
        return a * (b / GreatestCommonDenominator(a, b));
    }
}
