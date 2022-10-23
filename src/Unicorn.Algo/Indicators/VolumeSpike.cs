namespace Unicorn.Algo.Indicators;

public sealed class VolumeSpike : IIndicator<decimal, bool?>
{
    private readonly int _multiplier;
    private readonly StandardDeviation _stdDev;

    public VolumeSpike(int period, int multiplier)
    {
        if (period <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "The period cannot be less than or equal to 1");
        }

        if (multiplier < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "The multiplier cannot be less than 1");
        }

        _multiplier = multiplier;
        _stdDev = new StandardDeviation(period);
    }

    public bool IsReady => _stdDev.IsReady;

    public List<decimal> Source => _stdDev.Source;

    public bool? ComputeNextValue(decimal source)
    {
        var stdDev = _stdDev.ComputeNextValue(source);

        if (!_stdDev.IsReady)
        {
            return null;
        }

        var avg = Source.Average();
        var eq = avg + stdDev * _multiplier;
        return source > eq;
    }

    public void Reset()
    {
        _stdDev.Reset();
    }
}
