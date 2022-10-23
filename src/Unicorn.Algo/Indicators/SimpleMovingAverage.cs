namespace Unicorn.Algo.Indicators;

public sealed class SimpleMovingAverage : IIndicator<decimal, decimal>
{
    private readonly int _period;
    private readonly Queue<decimal> _queue = new();

    public SimpleMovingAverage(int period)
    {
        if (period <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "The period cannot be less than or equal to 1");
        }

        _period = period;
    }

    public bool IsReady => _queue.Count == _period;

    public List<decimal> Source => _queue.ToList();

    public decimal ComputeNextValue(decimal source)
    {
        _queue.Enqueue(source);

        if (_queue.Count > _period)
        {
            _queue.Dequeue();
        }

        var items = _queue.ToArray();
        return _queue.Count < _period ? 0 : items.Sum() / _queue.Count;
    }

    public void Reset()
    {
        _queue.Clear();
    }
}
