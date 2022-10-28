using Bet.Extensions.Testing.Logging;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Moedim.Microservices.FunctionalTest;

public class DemoWebApplicationFactory : WebApplicationFactory<Program>
{
    private ITestOutputHelper? _output;

    public void SetOutput(ITestOutputHelper output)
    {
        _output = output;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory());

        builder.ConfigureAppConfiguration((context, builder) =>
        {
            var dic = new Dictionary<string, string>
            {
                { "ASPNETCORE_ENVIRONMENT", "Development" },
                { "UserStore:Users:0:ApiKey", Constants.ApiKey },
                { "UserStore:Users:0:Roles:0", "ApiUser" },
            };

            builder.AddInMemoryCollection(dic!);
        });

        // override services.
        builder.ConfigureServices((builder, services) =>
        {
            services.AddLogging(x => x.ClearProviders().AddConsole().AddXunit(_output));
        });
    }
}
