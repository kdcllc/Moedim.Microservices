using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moedim.Microservices.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class HealthChecksBuilderExtensions
{
    /// <summary>
    /// Add SIGTERM Healcheck that provides notification for orchestrator
    /// with unhealthy status once the application begins to shut down.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the HealthCheck.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/>The type should be reported when the health check fails. Optional. If null then default is HealthStatus.UnHealthy.</param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static IHealthChecksBuilder AddSigtermCheck(
        this IHealthChecksBuilder builder,
        string name,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        builder.AddCheck<SigtermHealthCheck>(name, failureStatus, tags, timeout);

        return builder;
    }
}
