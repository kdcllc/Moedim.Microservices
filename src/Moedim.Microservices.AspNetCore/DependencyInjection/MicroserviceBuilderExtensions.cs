using Azure.Identity;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class MicroserviceBuilderExtensions
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
        var blobUri = $"{options.DataProtection.AzureBlobStorageUrl}/{options.DataProtection.ContainerName}/{options.DataProtection.FileName}";
        builder.Services
                .AddDataProtection()
                .PersistKeysToAzureBlobStorage(new Uri(blobUri), new DefaultAzureCredential());

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

        // adds blob healthcheck for DataProtection storage
        healthChecks.AddAzureBlobStorage(
            options.DataProtection.ContainerName,
            healthCheckOptions: (o, sp) =>
            {
                o.BlobServiceUri = options.DataProtection.AzureBlobStorageUrl;
                o.ContainerName = options.DataProtection.ContainerName;
            },
            failureStatus: HealthStatus.Degraded,
            tags: new string[] { "healthcheck", "azureBlobStorage" });

        healthChecks.AddSigtermCheck("appGracefulShutdown");

        healthChecks.AddAzureVaultSecrets(
            "azureVaults",
            (o, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                o.VaultUris = options.AzureVault.BaseUrl;
                o.Secrets.Add(options.AzureVault.HealthCheckSecret);
            },
            tags: new string[] { "healthcheck", "azureKeyVault" });

        // TODO: add healthchecks specifics
        // 3. azure database
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return builder;
    }

    public static IMicroserviceBuilder AddApplicationInsightsTelemetry(this IMicroserviceBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry(o => o.ConnectionString = builder.Options.ApplicationInsights.ConnectionString);

        return builder;
    }

    public static IMicroserviceBuilder AddProblemDetails(
       this IMicroserviceBuilder builder,
       Action<Hellang.Middleware.ProblemDetails.ProblemDetailsOptions> configure)
    {
        builder.Services.AddProblemDetails(
            options =>
            {
                options.IncludeExceptionDetails = (ctx, ex) => builder.Options.IncludeExceptionDetails;

                options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
                options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

                configure?.Invoke(options);

                options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            }); // Add the required services

        return builder;
    }
}
