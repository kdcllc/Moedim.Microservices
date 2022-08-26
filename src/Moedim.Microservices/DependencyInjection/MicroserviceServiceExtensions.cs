using Microsoft.Extensions.Configuration;

using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class MicroserviceServiceExtensions
{
    /// <summary>
    /// Creates <see cref="IMicroserviceBuilder"/> to be used for adding other Microservice components.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> passed from Host.</param>
    /// <param name="sectionName">The section name for <see cref="MicroserviceOptions"/> options.</param>
    /// <param name="configure">The action to configure <see cref="MicroserviceOptions"/>.</param>
    /// <returns></returns>
    public static IMicroserviceBuilder AddMicroservice(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Microservice",
        Action<MicroserviceOptions>? configure = default)
    {
        var options = configuration.GetOptions(sectionName, configure);

        // adds options to configure the microservice
        var optionsBuilder = services
            .AddOptions<MicroserviceOptions>()
            .ValidateDataAnnotations()
            .Configure<IConfiguration>((o, config) =>
            {
                config.Bind(sectionName, o);
                o.AzureLogAnalytics.ApplicationName = o.ServiceName;
                configure?.Invoke(o);
            });

        return new MicroserviceBuilder(services, options);
    }

    public static MicroserviceOptions GetOptions(
        this IConfiguration configuration,
        string sectionName,
        Action<MicroserviceOptions>? configure)
    {
        var options = new MicroserviceOptions();
        configuration.Bind(sectionName, options);
        configure?.Invoke(options);
        return options;
    }
}
