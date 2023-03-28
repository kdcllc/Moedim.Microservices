using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Moedim.Microservices.Options;

using Serilog;
using Serilog.Events;

namespace Microsoft.AspNetCore.Builder;

public static class MoedimApplicationBuilderExtensions
{
    /// <summary>
    /// Custom use redirect.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHttpsRedirect(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptionsMonitor<MicroserviceOptions>>();
        if (options.CurrentValue.HttpsEnabled)
        {
            app.UseHttpsRedirection();
        }

        return app;
    }

    public static IApplicationBuilder UseMoedimLogging(this IApplicationBuilder app)
    {
        app.UseHttpLogging();

        var options = app.ApplicationServices.GetRequiredService<IOptionsMonitor<MicroserviceOptions>>();

        if (options.CurrentValue.SerilogEnabled)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = LogHelper.GetLevel(LogEventLevel.Debug, "Health checks")!;
            });
        }

        return app;
    }
}
