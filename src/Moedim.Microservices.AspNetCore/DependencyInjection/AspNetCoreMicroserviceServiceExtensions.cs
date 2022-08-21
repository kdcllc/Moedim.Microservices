using Azure.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
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
    /// <param name="options"></param>
    /// <returns></returns>
    public static IMicroserviceBuilder AddDataProtection(
        this IMicroserviceBuilder builder,
        MicroserviceOptions? options = null)
    {
        options ??= builder.Options;

        builder.Services
        .AddDataProtection()
        .PersistKeysToAzureBlobStorage(new Uri($"{options.DataProtection.AzureBlobStorageUrl}/{options.DataProtection.ContainerName}/{options.DataProtection.FileName}"), new DefaultAzureCredential());

        return builder;
    }

    /// <summary>
    /// Adds default healthchecks, headers forwarding.
    ///
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IMicroserviceBuilder AddContainerSupport(
        this IMicroserviceBuilder builder,
        MicroserviceOptions? options = null)
    {
        options ??= builder.Options;

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
}
