namespace Unicorn.Algo.Indicators;

public interface IIndicator<TInput, out TOutput> where TInput : notnull
{
    bool IsReady { get; }

    List<TInput> Source { get; }

    TOutput ComputeNextValue(TInput source);

    void Reset();
}
