using System.Reflection;
using Xunit.Sdk;

namespace Unicorn.Tests.Algo.Unit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class CsvDataAttribute : DataAttribute
{
    private readonly string _filePath;
    private readonly bool _hasHeaders;

    /// <summary>
    /// Load data from a CSV file as the data source for a theory.
    /// </summary>
    /// <param name="filePath">The absolute or relative path to the CSV file to load.</param>
    /// <param name="hasHeaders">Whether the CSV file has headers or not.</param>
    public CsvDataAttribute(string filePath, bool hasHeaders)
    {
        _filePath = filePath;
        _hasHeaders = hasHeaders;
    }

    public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        ArgumentNullException.ThrowIfNull(methodInfo);

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"Could not find file at path {_filePath}");
        }

        var methodParameters = methodInfo.GetParameters();
        var parameterTypes = methodParameters.Select(x => x.ParameterType).ToArray();

        using var streamReader = new StreamReader(_filePath);

        if (_hasHeaders)
        {
            streamReader.ReadLine();
        }

        while (streamReader.ReadLine() is { } csvLine)
        {
            var csvRow = csvLine.Split(',');
            yield return ConvertCsv(csvRow, parameterTypes);
        }
    }

    private static object[] ConvertCsv(IReadOnlyList<object> csvRow, IReadOnlyList<Type> parameterTypes)
    {
        var convertedObject = new object[parameterTypes.Count];

        for (var i = 0; i < parameterTypes.Count; i++)
        {
            convertedObject[i] = parameterTypes[i] == typeof(int)
                ? Convert.ToInt32(csvRow[i])
                : csvRow[i];
        }

        return convertedObject;
    }
}
