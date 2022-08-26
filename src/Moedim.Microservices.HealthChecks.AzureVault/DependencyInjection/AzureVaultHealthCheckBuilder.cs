using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moedim.Microservices.HealthChecks.AzureVault;
using Moedim.Microservices.HealthChecks.AzureVault.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class AzureVaultHealthCheckBuilder
{
    public static IHealthChecksBuilder AddAzureVaultSecrets(
    this IHealthChecksBuilder builder,
    string name,
    Action<AzureKeyVaultOptions, IServiceProvider> healthCheckOptions,
    HealthStatus? failureStatus = default,
    IEnumerable<string>? tags = default,
    TimeSpan? timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(
           name,
           sp =>
           {
               var options = new AzureKeyVaultOptions();
               healthCheckOptions(options, sp);
               options.Name = name;

               return new AzureKeyVaultHealthCheck(options);
           },
           failureStatus,
           tags,
           timeout));
    }
}
