using Serilog;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationLoggingDebugExtensions
{
    /// <summary>
    /// Displays all of the application configurations based on the Configuration Provider.
    /// </summary>
    /// <param name="config"></param>
    public static void DebugViewSerilogConfiguration(this IConfigurationRoot config)
    {
        var logger = new LoggerConfiguration()
                                          .MinimumLevel.Debug()
                                          .WriteTo.Debug()
                                          .WriteTo.Console()
                                          .CreateLogger();

        logger.Debug(config.GetDebugView());
    }
}
