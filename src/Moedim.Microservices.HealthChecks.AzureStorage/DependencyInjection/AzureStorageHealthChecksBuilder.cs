using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moedim.Microservices.HealthChecks.AzureStorage;
using Moedim.Microservices.HealthChecks.AzureStorage.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureStorageHealthChecksBuilder
    {
        public static IHealthChecksBuilder AddAzureBlobStorage(
            this IHealthChecksBuilder builder,
            string name,
            Action<AzureBlobStorageHealthCheckOptions, IServiceProvider> healthCheckOptions,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            return builder.Add(new HealthCheckRegistration(
               name,
               sp =>
               {
                   var options = new AzureBlobStorageHealthCheckOptions();
                   healthCheckOptions(options, sp);

                   return new AzureBlobStorageHealthCheck(options);
               },
               failureStatus,
               tags,
               timeout));
        }
    }
}
