using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moedim.Microservices.Options;

using Serilog;
using Serilog.Exceptions;

namespace Microsoft.Extensions.Hosting;

public static class MicroservicesHostBuilderExtensions
{
    /// <summary>
    /// Adds Azure Vault Secrets based on Environemnt.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IHostBuilder AddAzureVault(
        this IHostBuilder hostBuilder,
        MicroserviceOptions options,
        string sectionName = "Microservice:AzureVault")
    {
        hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
        {
            if (options.AzureVaultEnabled)
            {
                // based on environment Development = dev; Production = prod prefix in Azure Vault.
                var envName = hostingContext.HostingEnvironment.EnvironmentName;

                var configuration = configBuilder.AddAzureKeyVault(
                    hostingEnviromentName: envName,
                    sectionName: sectionName,
                    reloadInterval: TimeSpan.FromSeconds(30));

                // helpful to see what was retrieved from all of the configuration providers.
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    configuration.Build().DebugViewSerilogConfiguration();
                }
            }
        });

        return hostBuilder;
    }

    /// <summary>
    /// Adds Serilog Logging with Azure Log Analytics if the configuration is present.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="options"></param>
    /// <param name="azureLogAnalyticsSection"></param>
    /// <param name="applicationInsightsSection"></param>
    /// <param name="version">The version of the logs property.</param>
    /// <returns></returns>
    public static IHostBuilder AddSerilogLogging(
        this IHostBuilder hostBuilder,
        MicroserviceOptions options,
        string azureLogAnalyticsSection = "Microservice:AzureLogAnalytics",
        string applicationInsightsSection = "Microservice:ApplicationInsights",
        string version = "1.0.0")
    {
        if (applicationInsightsSection is null)
        {
            throw new ArgumentNullException(nameof(applicationInsightsSection));
        }

        hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var appName = $"{options.ServiceName}{hostingContext.HostingEnvironment.EnvironmentName}";
            loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Version", version)
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .Enrich.WithExceptionDetails()
                    .AddAzureLogAnalytics(
                        hostingContext.Configuration,
                        configure: (o) => o.ApplicationName = appName,
                        sectionName: azureLogAnalyticsSection);

            if (options.ApplicationInsightsEnabled)
            {
                loggerConfiguration.AddApplicationInsightsTelemetry(
                    configuration: hostingContext.Configuration,
                    sectionName: applicationInsightsSection);
            }
        });

        return hostBuilder;
    }
}
