using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Moedim.Microservices.UnitTest.Logging;

public class AppInsightWorkerTests
{
    [Fact(Skip = "Local Testing")]
    public void ApplicationInsightsServiceOptions_ConnectionString_Set()
    {
        var services = new ServiceCollection();

        var configBuilder = new ConfigurationBuilder();

        var dict = new Dictionary<string, string?>
        {
            { "ApplicationInsights:ConnectionString", "string" }
        };

        configBuilder.AddInMemoryCollection(dict);
        var config = configBuilder.Build();

        services.AddSingleton<IConfiguration>(config);

        services.AddApplicationInsightsTelemetry();

        var sp = services.BuildServiceProvider();

        var tc = sp.GetRequiredService<TelemetryClient>();

        Assert.NotNull(tc);
    }
}
