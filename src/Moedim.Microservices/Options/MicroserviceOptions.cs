using System.ComponentModel.DataAnnotations;

namespace Moedim.Microservices.Options;

/// <summary>
/// The basic options for microservices.
/// </summary>
public class MicroserviceOptions
{
    [Required]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    public bool HttpsEnabled { get; set; }

    [Required]
    public bool AzureVaultEnabled { get; set; }

    [Required]
    public bool SerilogEnabled { get; set; }

    [Required]
    public bool IncludeExceptionDetails { get; set; }

    public bool ApplicationInsightsEnabled { get; set; }

    public AzureVaultOptions AzureVault { get; set; } = new AzureVaultOptions();

    [Required]
    public DataProtectionOptions DataProtection { get; set; } = new DataProtectionOptions();

    public ApplicationInsightsOptions ApplicationInsights { get; set; } = new ApplicationInsightsOptions();

    public AzureLogAnalyticsOptions AzureLogAnalytics { get; set; } = new AzureLogAnalyticsOptions();

    public string DefaultAllCorsAllowedPolicyName { get; set; } = nameof(DefaultAllCorsAllowedPolicyName);
}
