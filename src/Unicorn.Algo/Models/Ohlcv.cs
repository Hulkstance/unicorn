using System.Diagnostics.CodeAnalysis;

namespace Unicorn.Algo.Models;

public sealed class Ohlcv : IEquatable<Ohlcv>
{
    public Ohlcv(DateTimeOffset date, decimal open, decimal high, decimal low, decimal close, decimal volume)
        => (Date, Open, High, Low, Close, Volume) = (date, open, high, low, close, volume);

    public DateTimeOffset Date { get; }
    public decimal Open { get; }
    public decimal High { get; }
    public decimal Low { get; }
    public decimal Close { get; }
    public decimal Volume { get; }

    public override string ToString()
    {
        return $"Date: {Date}, Open: {Open}, High: {High}, Low: {Low}, Close: {Close}, Volume: {Volume}";
    }

    #region Tester-Doer Pattern

    public static Ohlcv CsvParse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            throw new ArgumentNullException(nameof(line));
        }

        var cells = line.Split(',');

        var date = DateTimeOffset.FromUnixTimeSeconds(ParseInt64(cells[0]));
        var open = ParseDecimal(cells[1]);
        var high = ParseDecimal(cells[2]);
        var low = ParseDecimal(cells[3]);
        var close = ParseDecimal(cells[4]);
        var volume = ParseDecimal(cells[5]);
        return new Ohlcv(date, open, high, low, close, volume);
    }

    public static bool TryCsvParse(string line, [NotNullWhen(returnValue: true)] out Ohlcv? candle)
    {
        try
        {
            candle = CsvParse(line);
            return true;
        }
        catch
        {
            candle = null;
            return false;
        }
    }

    private static long ParseInt64(string s)
    {
        if (long.TryParse(s, out var x))
        {
            return x;
        }

        throw new InvalidOperationException("Unable to parse to long");
    }

    private static decimal ParseDecimal(string s)
    {
        if (decimal.TryParse(s, out var x))
        {
            return x;
        }

        throw new InvalidOperationException("Unable to parse to decimal");
    }

    #endregion

    #region IEquatable

    public bool Equals(Ohlcv? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Date.Equals(other.Date) && Open == other.Open && High == other.High && Low == other.Low &&
               Close == other.Close && Volume == other.Volume;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Ohlcv other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, Open, High, Low, Close, Volume);
    }

    #endregion
}
