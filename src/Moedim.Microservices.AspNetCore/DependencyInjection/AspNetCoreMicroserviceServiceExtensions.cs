using Azure.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class AspNetCoreMicroserviceServiceExtensions
{
    /// <summary>
    /// Adds Azure Blob Storage for Data Protection Keys.
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers"/>
    /// and
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/overview/azure/extensions.aspnetcore.dataprotection.blobs-readme"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMicroserviceBuilder AddDataProtection(this IMicroserviceBuilder builder)
    {
        var options = builder.Options;

        builder.Services
        .AddDataProtection()
        .PersistKeysToAzureBlobStorage(new Uri($"{options.DataProtection.AzureBlobStorageUrl}/{options.DataProtection.ContainerName}/{options.DataProtection.FileName}"), new DefaultAzureCredential());

        return builder;
    }

    /// <summary>
    /// <para>Adds default healthchecks, headers forwarding.</para>
    /// <para><see href="https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer"/>.</para>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMicroserviceBuilder AddContainerSupport(this IMicroserviceBuilder builder)
    {
        var options = builder.Options;

        var healthChecks = builder.Services.AddHealthChecks();

        healthChecks.AddAzureBlobStorage(
            options.DataProtection.ContainerName,
            healthCheckOptions: (o, sp) =>
            {
                o.BlobServiceUri = options.DataProtection.AzureBlobStorageUrl;
                o.ContainerName = options.DataProtection.ContainerName;
            },
            failureStatus: HealthStatus.Degraded,
            tags: new string[] { "blobStorage" });

        healthChecks.AddSigtermCheck("appGracefulShutdown");

        // TODO: add healthchecks specifics
        // 1. azure vault
        // 2. azure blob storage
        // 3. azure database

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return builder;
    }

    public static IMicroserviceBuilder AddApplicationInsightsTelemetry(
        this IMicroserviceBuilder builder,
        string sectionName = "ApplicationInsights",
        Action<ApplicationInsightsOptions, IConfiguration>? configure = null)
    {
        builder.Services.AddApplicationInsightsTelemetry(sectionName, configure);
        builder.Services.AddApplicationInsightsTelemetry();

        return builder;
    }
}
