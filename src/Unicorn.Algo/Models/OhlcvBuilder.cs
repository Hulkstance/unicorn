namespace Unicorn.Algo.Models;

public sealed class OhlcvBuilder
{
    private DateTimeOffset _date;
    private decimal _open;
    private decimal _high;
    private decimal _low;
    private decimal _close;
    private decimal _volume;

    public Ohlcv Build()
    {
        return new Ohlcv(_date, _open, _high, _low, _close, _volume);
    }

    public OhlcvBuilder WithDate(DateTimeOffset date)
    {
        _date = date;
        return this;
    }

    public OhlcvBuilder WithOpen(decimal open)
    {
        if (open < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(open), "The open cannot be null.");
        }

        _open = open;
        return this;
    }

    public OhlcvBuilder WithHigh(decimal high)
    {
        if (high < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(high), "The high cannot be null.");
        }

        _high = high;
        return this;
    }

    public OhlcvBuilder WithLow(decimal low)
    {
        if (low < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(low), "The low cannot be null.");
        }

        _low = low;
        return this;
    }

    public OhlcvBuilder WithClose(decimal close)
    {
        if (close < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(close), "The close cannot be null.");
        }

        _close = close;
        return this;
    }

    public OhlcvBuilder WithVolume(decimal volume)
    {
        if (volume < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(volume), "The volume cannot be null.");
        }

        _volume = volume;
        return this;
    }
}
