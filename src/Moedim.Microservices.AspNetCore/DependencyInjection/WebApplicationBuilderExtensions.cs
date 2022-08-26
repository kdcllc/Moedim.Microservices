using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Moedim.Microservices.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApplicationBuilderExtensions
{
    public static IMicroserviceBuilder AddMicroServices(
                this WebApplicationBuilder hostBuilder,
                Action<MicroserviceOptions> configure,
                string sectionName = "Microservice")
    {
        // bind existing configurations
        var options = hostBuilder.Configuration.GetOptions(sectionName, configure);

        // set app azure
        hostBuilder.Host.AddAzureVault(options);

        var env = hostBuilder.Environment.EnvironmentName;
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddConfiguration(hostBuilder.Configuration);

        configBuilder.AddAzureKeyVault(hostingEnviromentName: env, sectionName: $"{sectionName}:AzureVault");

        var config = configBuilder.Build();

        var builder = hostBuilder.Services
                        .AddMicroservice(config, sectionName, configure);

        if (builder.Options.SerilogEnabled)
        {
            hostBuilder.Host.AddSerilogLogging(builder.Options);
        }
        else
        {
            builder.Services.AddAzureLogAnalytics(
                hostBuilder.Configuration,
                configure: o => o.ApplicationName = $"{builder.Options.ServiceName.KeepAllLetters()}{hostBuilder.Environment.EnvironmentName}",
                sectionName: "Microservice:AzureLogAnalytics",
                filter: (s, l) => true);
        }

        return builder;
    }
}
