using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Builder;

public static class HealthChecksEndpointConventionBuilderExtensions
{
    /// <summary>
    /// <para>
    /// Map Healtheck Liveleness route.
    /// Enable usage of the basic liveness check that returns 200 http status code.
    /// Default registered health check is self.
    /// </para>
    /// <para>Liveness indicates if an app has crashed and must be restarted.</para>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="healthCheckPath"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapLivenessHealthCheck(
        this IEndpointRouteBuilder builder,
        string healthCheckPath = "/liveness",
        Action<HealthCheckOptions>? configure = null)
    {
        var options = new HealthCheckOptions
        {
            Predicate = _ => false
        };

        configure?.Invoke(options);

        builder.MapHealthChecks(healthCheckPath, options);

        return builder;
    }

    /// <summary>
    /// <para>Use Healthcheck which returns a report of all registered healthchecks.</para>
    /// <para>Readiness indicates if the app is running normally but isn't ready to receive requests.</para>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="healthCheckPath"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapHealthyHealthCheck(
        this IEndpointRouteBuilder builder,
        string healthCheckPath = "/healthy",
        Action<HealthCheckOptions>? configure = default)
    {
        var options = new HealthCheckOptions
        {
            ResponseWriter = WriteResponseAsync
        };

        configure?.Invoke(options);
        builder.MapHealthChecks(healthCheckPath, options);

        return builder;
    }

    /// <summary>
    /// Custom HealthCheck delegate that write a detailed response.
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks#customize-output"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="healthReport"></param>
    /// <returns></returns>
    public static Task WriteResponseAsync(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description", healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
