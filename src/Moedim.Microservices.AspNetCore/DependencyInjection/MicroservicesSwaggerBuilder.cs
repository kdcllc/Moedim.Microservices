namespace Microsoft.Extensions.DependencyInjection;

public class MicroservicesSwaggerBuilder : IMicroservicesSwaggerBuilder
{
    public MicroservicesSwaggerBuilder(
        IServiceCollection services,
        bool enableVersioning)
    {
        Services = services;
        EnableSwagerVersionSupport = enableVersioning;
    }

    public IServiceCollection Services { get; }

    public bool EnableSwagerVersionSupport { get; }
}
