using System.Globalization;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Unicorn.Logging;

public static class LoggingExtensions
{
    private const string ConsoleOutputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";
    private const string FileOutputTemplate = "{Message:lj}{NewLine}{Exception}";

    public static IHostBuilder ConfigureLogging(this IHostBuilder builder, bool writeToFile = true)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseSerilog((context, provider, configuration) =>
            ConfigureLogging(context, provider, configuration, writeToFile));

        return builder;
    }

    private static void ConfigureLogging(
        HostBuilderContext hostBuilderContext,
        IServiceProvider serviceProvider,
        LoggerConfiguration loggerConfiguration,
        bool writeToFile = true)
    {
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("System", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: ConsoleOutputTemplate);

        if (writeToFile)
        {
            var formatter = new MessageTemplateTextFormatter(FileOutputTemplate, CultureInfo.InvariantCulture);
            loggerConfiguration.WriteFilteredToRollingFile(formatter, "logs", LogEventLevel.Information);
        }
    }

    private static void WriteFilteredToRollingFile(
        this LoggerConfiguration loggerConfiguration,
        ITextFormatter formatter,
        string path,
        LogEventLevel logEventLevel,
        int bufferSize = 10000,
        bool blockWhenFull = false,
        int? retainedFileCountLimit = 24,
        long? fileSizeLimitBytes = 1L * 1024 * 1024 * 100)
    {
        loggerConfiguration.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
            .WriteTo.Async(lsc => lsc.File(formatter,
                    Path.Combine(path, "mkt-data.txt"),
                    rollingInterval: RollingInterval.Hour,
                    retainedFileCountLimit: retainedFileCountLimit,
                    fileSizeLimitBytes: fileSizeLimitBytes,
                    rollOnFileSizeLimit: true),
                bufferSize, blockWhenFull));
    }
}
