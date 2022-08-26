using System.Collections.Concurrent;

using Azure.Security.KeyVault.Secrets;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moedim.Microservices.HealthChecks.AzureVault.Options;

namespace Moedim.Microservices.HealthChecks.AzureVault
{
    public class AzureKeyVaultHealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<Uri, SecretClient> _store = new();

        private readonly AzureKeyVaultOptions _options;

        public AzureKeyVaultHealthCheck(AzureKeyVaultOptions options)
        {
            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var vault in _options.VaultUris.Split(";"))
                {
                    foreach (var secret in _options.Secrets)
                    {
                        var secretClient = CreateSecretClient(vault);
                        await secretClient.GetSecretAsync(secret, cancellationToken: cancellationToken);
                    }
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private SecretClient CreateSecretClient(string vaultUri)
        {
            if (!_store.TryGetValue(new Uri(vaultUri), out var client))
            {
                client = new SecretClient(new Uri(_options.VaultUris), _options.Credential);
                _store.TryAdd(new Uri(vaultUri), client);
            }

            return client;
        }
    }
}
