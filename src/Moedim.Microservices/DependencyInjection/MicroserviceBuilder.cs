using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public class MicroserviceBuilder : IMicroserviceBuilder
{
    public MicroserviceBuilder(IServiceCollection services, MicroserviceOptions options)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public IServiceCollection Services { get; set; }

    public MicroserviceOptions Options { get; set; }
}
