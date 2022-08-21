using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moedim.Microservices.HealthChecks;

namespace Moedim.Microservices.UnitTest.HealthChecks;

public class SigtermHealthCheckTests
{
    [Fact]
    public void Add_HealthCheck_Successfully()
    {
        var services = new ServiceCollection();

        var checkName = "appStopping";

        services.AddSingleton<IHostApplicationLifetime, TestHostApplicationLifeTime>();
        services.AddHealthChecks().AddSigtermCheck(checkName);

        var sp = services.BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.First();
        var check = registration.Factory(sp);

        Assert.Equal(checkName, registration.Name);
        Assert.Equal(typeof(SigtermHealthCheck), check.GetType());
    }

    [Fact]
    public void Stopping_HealthCheck_Successfully()
    {
        var services = new ServiceCollection();

        var checkName = "appStopping";

        services.AddSingleton<IHostApplicationLifetime, TestHostApplicationLifeTime>();
        services.AddHealthChecks().AddSigtermCheck(checkName);

        var sp = services.BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.First();
        var check = registration.Factory(sp);

        var lifeTime = sp.GetRequiredService<IHostApplicationLifetime>();
        lifeTime.StopApplication();

        Assert.Equal(HealthStatus.Unhealthy, registration.FailureStatus);
    }
}
