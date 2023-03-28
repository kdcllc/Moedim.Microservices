using Microsoft.AspNetCore.Http;

namespace Serilog.Events;

public static class LogHelper
{
    /// <summary>
    /// Create a <see cref="Serilog.AspNetCore.RequestLoggingOptions.GetLevel"/> method that
    /// uses the default logging level, except for the specified endpoint names, which are
    /// logged using the provided <paramref name="traceLevel" />.
    /// </summary>
    /// <param name="traceLevel">The level to use for logging "trace" endpoints.</param>
    /// <param name="traceEndointNames">The display name of endpoints to be considered "trace" endpoints.</param>
    /// <returns></returns>
    public static Func<HttpContext, double, Exception, LogEventLevel> GetLevel(
        LogEventLevel traceLevel,
        params string[] traceEndointNames)
    {
        if (traceEndointNames is null || traceEndointNames.Length == 0)
        {
            throw new ArgumentNullException(nameof(traceEndointNames));
        }

        return (ctx, _, ex) =>
            IsError(ctx, ex)
            ? LogEventLevel.Error
            : IsTraceEndpoint(ctx, traceEndointNames)
                ? traceLevel
        : LogEventLevel.Information;
    }

    private static bool IsError(HttpContext ctx, Exception ex)
    {
        return ex != null || ctx.Response.StatusCode > 499;
    }

    private static bool IsTraceEndpoint(HttpContext ctx, string[] traceEndoints)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is object)
        {
            for (var i = 0; i < traceEndoints.Length; i++)
            {
                if (string.Equals(traceEndoints[i], endpoint.DisplayName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
