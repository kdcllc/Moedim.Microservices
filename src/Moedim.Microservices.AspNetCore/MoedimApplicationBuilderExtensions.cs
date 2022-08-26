using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Moedim.Microservices.Options;

using Serilog;
using Serilog.Events;

namespace Microsoft.AspNetCore.Builder;

public static class MoedimApplicationBuilderExtensions
{
    /// <summary>
    /// Custome use redirect.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHttpsRedirect(this IApplicationBuilder app)
    {
        var enableHttpsRedirection = app.ApplicationServices.GetRequiredService<IOptionsMonitor<MicroserviceOptions>>();
        if (enableHttpsRedirection.CurrentValue.HttpsEnabled)
        {
            app.UseHttpsRedirection();
        }

        return app;
    }

    public static IApplicationBuilder UseMoedimLogging(this IApplicationBuilder app)
    {
        app.UseHttpLogging();

        app.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = LogHelper.GetLevel(LogEventLevel.Debug, "Health checks");
        });

        return app;
    }
}
