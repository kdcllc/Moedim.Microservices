using Microsoft.Extensions.Configuration;

using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class MicroserviceServiceExtensions
{
    public static IMicroserviceBuilder AddMicroservice(
        this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null,
        Action<MicroserviceOptions>? configure = default)
    {
        sectionName ??= nameof(MicroserviceOptions);

        var options = new MicroserviceOptions();
        configuration.Bind(sectionName, options);

        // adds options to configure the microservice
        var optionsBuilder = services
            .AddOptions<MicroserviceOptions>()
            .ValidateDataAnnotations();

        optionsBuilder.Configure(o =>
        {
            configure?.Invoke(o);

            o.AzureVaultEnabled = options.AzureVaultEnabled;
            o.HttpsEnabled = options.HttpsEnabled;
            o.DataProtection.ContainerName = options.DataProtection.ContainerName;
            o.DataProtection.FileName = options.DataProtection.FileName;
            o.DataProtection.AzureBlobStorageUrl = options.DataProtection.AzureBlobStorageUrl;
        });

        return new MicroserviceBuilder(services, options);
    }
}
