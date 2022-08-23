using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationLoggingDebugExtensions
{
    /// <summary>
    /// Displays all of the application configurations based on the Configuration Provider.
    /// </summary>
    /// <param name="config"></param>
    public static void DebugViewConfiguration(this IConfigurationRoot config)
    {
        using var logFactory = GetLoggerFactory(config);

        var logger = logFactory.CreateLogger("Program");

        logger.LogDebug(config.GetDebugView());
    }

    /// <summary>
    ///  Allows to create a Logger for debugging purposes.
    /// </summary>
    /// <returns></returns>
    public static ILoggerFactory GetLoggerFactory(IConfiguration? configuration = default)
    {
        return LoggerFactory.Create(builder =>
        {
            if (configuration != null)
            {
                builder.AddConfiguration();
            }

            builder.AddDebug();
            builder.AddConsole();
            builder.AddFilter((_) => true);
        });
    }
}
