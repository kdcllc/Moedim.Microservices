using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public interface IMicroserviceBuilder
{
    public IServiceCollection Services { get; set; }

    public MicroserviceOptions Options { get; set; }
}
