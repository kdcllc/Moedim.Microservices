using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Moedim.Microservices.Options;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtentions
{
    public static WebApplication UseHttpsRedirect(this WebApplication app)
    {
        var enableHttpsRedirection = app.Services.GetRequiredService<IOptionsMonitor<MicroserviceOptions>>();
        if (enableHttpsRedirection.CurrentValue.HttpsEnabled)
        {
            app.UseHttpsRedirection();
        }

        return app;
    }
}
