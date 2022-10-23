using Unicorn.Algo.Models;

namespace Unicorn.Tests.Algo.Unit.Extensions;

public static class CsvExtensions
{
    public static IEnumerable<Ohlcv> ReadCsv(this string filePath)
    {
        using var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line is null) continue;

            if (Ohlcv.TryCsvParse(line, out var ohlcv))
            {
                yield return ohlcv;
            }
        }
    }
}
