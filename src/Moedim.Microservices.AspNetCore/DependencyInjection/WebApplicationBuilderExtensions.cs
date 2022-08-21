using Azure.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddMicroService(
    this WebApplicationBuilder builder,
    string? sectionName = null,
    Action<MicroserviceOptions>? configure = null)
    {
        var b = builder.Services
                        .AddMicroservice(builder.Configuration, sectionName, configure)
                        .AddDataProtection()
                        .AddContainerSupport();

        builder.AddAzureVault(b.Options);

        return builder;
    }

    /// <summary>
    /// Adds Azure Vault Secrets based on Environemnt.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAzureVault(
        this WebApplicationBuilder builder,
        MicroserviceOptions options)
    {
        builder.WebHost.ConfigureAppConfiguration((hostingContext, configBuilder) =>
        {
            if (options.AzureVaultEnabled)
            {
                // based on environment Development = dev; Production = prod prefix in Azure Vault.
                var envName = hostingContext.HostingEnvironment.EnvironmentName;

                var configuration = configBuilder.AddAzureKeyVault(
                    hostingEnviromentName: envName,
                    reloadInterval: TimeSpan.FromSeconds(30));

                // helpful to see what was retrieved from all of the configuration providers.
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    // configuration.DebugConfigurations();
                }
            }
        });

        return builder;
    }

    /// <summary>
    /// Adds Azure Blob Storage for Data Protection Keys.
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers"/>
    /// and
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/overview/azure/extensions.aspnetcore.dataprotection.blobs-readme"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddDataProtection(
        this WebApplicationBuilder builder,
        MicroserviceOptions options)
    {
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
    public static WebApplicationBuilder AddContainerSupport(
        this WebApplicationBuilder builder,
        MicroserviceOptions options)
    {
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
