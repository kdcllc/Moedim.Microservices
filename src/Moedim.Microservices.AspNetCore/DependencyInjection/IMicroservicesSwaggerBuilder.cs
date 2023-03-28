namespace Microsoft.Extensions.DependencyInjection;

public interface IMicroservicesSwaggerBuilder
{
    public IServiceCollection Services { get; }

    internal bool EnableSwagerVersionSupport { get; }
}
