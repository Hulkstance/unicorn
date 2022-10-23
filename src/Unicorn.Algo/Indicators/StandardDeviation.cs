namespace Unicorn.Algo.Indicators;

public sealed class StandardDeviation : IIndicator<decimal, decimal>
{
    private readonly Variance _variance;

    public StandardDeviation(int period)
    {
        if (period <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "The period cannot be less than or equal to 1");
        }

        _variance = new Variance(period);
    }

    public bool IsReady => _variance.IsReady;

    public List<decimal> Source => _variance.Source;

    public decimal ComputeNextValue(decimal source)
    {
        var variance = _variance.ComputeNextValue(source);
        return (decimal)Math.Sqrt((double)variance);
    }

    public void Reset()
    {
        _variance.Reset();
    }
}
