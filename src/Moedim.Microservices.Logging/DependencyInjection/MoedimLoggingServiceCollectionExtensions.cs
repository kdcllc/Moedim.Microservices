using LogAnalytics.Extensions.Logging;

using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class MoedimLoggingServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationInsightsTelemetry(
        this IServiceCollection services,
        string sectionName = "ApplicationInsights",
        Action<ApplicationInsightsOptions, IConfiguration>? configure = null)
    {
        services.AddOptions<ApplicationInsightsOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    config.Bind(sectionName, options);
                    configure?.Invoke(options, config);
                });

        services.AddOptions<ApplicationInsightsServiceOptions>()
                .PostConfigure<IServiceProvider>((options, sp) =>
                {
                    var o = sp.GetRequiredService<IOptionsMonitor<ApplicationInsightsOptions>>().CurrentValue;
                    options.ConnectionString = o.ConnectionString;
                });

        services.AddApplicationInsightsTelemetryWorkerService();

        return services;
    }

    public static IServiceCollection AddAzureLogAnalytics(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AzureLogAnalyticsOptions>? configure = null,
        string? serviceNamespace = null,
        Func<string, LogLevel, bool>? filter = null,
        string sectionName = "AzureLogAnalytics")
    {
        var options = new AzureLogAnalyticsOptions();
        configuration.Bind(sectionName, options);
        configure?.Invoke(options);

        var appName = !string.IsNullOrEmpty(options.ApplicationName) ? options.ApplicationName : configuration[HostDefaults.ApplicationKey];

        services.AddLogging(l =>
        {
            l.AddProvider(new LogAnalyticsLoggerProvider(filter, options.WorkspaceId, options.AuthenticationId, appName, serviceNamespace));
        });
        return services;
    }
}
