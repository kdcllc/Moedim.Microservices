using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Moedim.Microservices.HealthChecks;

/// <summary>
/// HealthCheck for graceful shutdown.
/// </summary>
public class SigtermHealthCheck : IHealthCheck, IDisposable
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private CancellationTokenRegistration? _stoppingTokenRegistration = null;

    public SigtermHealthCheck(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        _stoppingTokenRegistration = _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_stoppingTokenRegistration != null ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }

    public void Dispose()
    {
        _stoppingTokenRegistration?.Dispose();
        _stoppingTokenRegistration = null;
    }

    private void OnStopping()
    {
        Dispose();
    }
}
