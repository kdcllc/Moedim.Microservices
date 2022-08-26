using Azure.Core;
using Azure.Identity;

namespace Moedim.Microservices.HealthChecks.AzureVault.Options;

public class AzureKeyVaultOptions
{
    public string VaultUris { get; set; } = string.Empty;

    public TokenCredential Credential { get; set; } = new DefaultAzureCredential();

    public IList<string> Secrets { get; set; } = new List<string>();

    internal string Name { get; set; } = string.Empty;
}
