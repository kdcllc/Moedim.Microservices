using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moedim.Microservices.Options;

namespace Serilog;

public static class LoggerConfigurationExtensions
{
    /// <summary>
    /// Adds Azure Log Analytics to the Serilog Sink.
    /// </summary>
    /// <param name="loggerConfiguration">The instance of LoggerConfiguration.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The options configuration section name.</param>
    /// <param name="configure">The action to confiture the options.</param>
    /// <param name="batchSize">The size of the batch to send to Azue Log Analytics.</param>
    /// <returns></returns>
    public static LoggerConfiguration AddAzureLogAnalytics(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        string sectionName = "AzureLogAnalytics",
        Action<AzureLogAnalyticsOptions>? configure = null,
        int batchSize = 10)
    {
        var options = new AzureLogAnalyticsOptions();
        configuration.Bind(sectionName, options);
        configure?.Invoke(options);

        if (!string.IsNullOrEmpty(options.WorkspaceId)
            && !string.IsNullOrEmpty(options.AuthenticationId))
        {
            var appName = !string.IsNullOrEmpty(options.ApplicationName) ? options.ApplicationName : configuration[HostDefaults.ApplicationKey];

            // write to Log Analytics
            loggerConfiguration.WriteTo.AzureAnalytics(
                options.WorkspaceId,
                options.AuthenticationId,
                logName: appName?.KeepAllLetters(),
                batchSize: batchSize);
        }

        return loggerConfiguration;
    }

    /// <summary>
    /// Adds Azure ApplicationInsights Serilog Sink.
    /// </summary>
    /// <param name="loggerConfiguration">The instance of LoggerConfiguration.</param>
    /// <param name="provider">The configuration instance.</param>
    /// <param name="sectionName">The options configuration section name.</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static LoggerConfiguration AddApplicationInsightsTelemetry(
        this LoggerConfiguration loggerConfiguration,
        IServiceProvider provider,
        string sectionName = "ApplicationInsights",
        Action<ApplicationInsightsOptions>? configure = null)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        // writes to Application Insights window
        var options = new ApplicationInsightsOptions();
        configuration.Bind(sectionName, options);
        configure?.Invoke(options);

        if (!string.IsNullOrEmpty(options.ConnectionString))
        {
            var telemteryConfiguration = provider.GetService<TelemetryConfiguration>();

            if (telemteryConfiguration == null)
            {
                telemteryConfiguration = new TelemetryConfiguration
                {
                    ConnectionString = options.ConnectionString
                };
            }

            if (options.EnableEvents)
            {
                loggerConfiguration.WriteTo.ApplicationInsights(telemteryConfiguration, TelemetryConverter.Events);
            }

            if (options.EnableTraces)
            {
                loggerConfiguration.WriteTo.ApplicationInsights(telemteryConfiguration, TelemetryConverter.Traces);
            }
        }

        return loggerConfiguration;
    }

    /// <summary>
    /// Adds Azure ApplicationInsights Serilog Sink.
    /// </summary>
    /// <param name="loggerConfiguration">The instance of LoggerConfiguration.</param>
    /// <param name="configuration"></param>
    /// <param name="sectionName">The options configuration section name.</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static LoggerConfiguration AddApplicationInsightsTelemetry(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        string sectionName = "ApplicationInsights",
        Action<ApplicationInsightsOptions>? configure = null)
    {
        // writes to Application Insights window
        var options = new ApplicationInsightsOptions();
        configuration.Bind(sectionName, options);
        configure?.Invoke(options);

        if (!string.IsNullOrEmpty(options.ConnectionString))
        {
            // https://docs.microsoft.com/en-us/azure/azure-monitor/app/console#getting-started
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.ConnectionString = options.ConnectionString;

            if (options.EnableEvents)
            {
                loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Events);
            }

            if (options.EnableTraces)
            {
                loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
            }
        }

        return loggerConfiguration;
    }
}
