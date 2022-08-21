using System.ComponentModel.DataAnnotations;

namespace Moedim.Microservices.Options;

/// <summary>
/// The basic options for microservices.
/// </summary>
public class MicroserviceOptions
{
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool AzureVaultEnabled { get; set; }

    [Required]
    public bool HttpsEnabled { get; set; }

    [Required]
    public DataProtectionOptions DataProtection { get; set; } = new DataProtectionOptions();
}
