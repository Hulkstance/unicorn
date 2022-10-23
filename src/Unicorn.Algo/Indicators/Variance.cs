using Unicorn.Algo.Indicators.Utility;

namespace Unicorn.Algo.Indicators;

public sealed class Variance : IIndicator<decimal, decimal>
{
    private readonly RollingWindow<decimal> _window;
    private decimal _rollingSum;
    private decimal _rollingSumOfSquares;

    public Variance(int period)
    {
        if (period <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "The period cannot be less than or equal to 1");
        }

        _window = new RollingWindow<decimal>(period);
    }

    public bool IsReady => _window.IsReady;

    public List<decimal> Source => _window.Source;

    public decimal ComputeNextValue(decimal source)
    {
        _window.Add(source);

        var asd = _window.ToString();

        _rollingSum += source;
        _rollingSumOfSquares += source * source;

        if (_window.Count < _window.WindowSize)
        {
            return 0;
        }

        var meanValue1 = _rollingSum / _window.WindowSize;
        var meanValue2 = _rollingSumOfSquares / _window.WindowSize;

        // Drop head / drop oldest
        var removedValue = _window[0];
        _rollingSum -= removedValue;
        _rollingSumOfSquares -= removedValue * removedValue;

        return meanValue2 - (meanValue1 * meanValue1);
    }

    public void Reset()
    {
        _window.Clear();

        _rollingSum = 0;
        _rollingSumOfSquares = 0;
    }
}
