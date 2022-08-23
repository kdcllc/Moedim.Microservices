namespace Moedim.Microservices.Options;

public class ApplicationInsightsOptions
{
    /// <summary>
    /// Instrumentation Connection String.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Enables Event tracing.
    /// </summary>
    public bool EnableEvents { get; set; } = true;

    /// <summary>
    /// Enables Traces with Azure AppInsights.
    /// </summary>
    public bool EnableTraces { get; set; } = true;
}
