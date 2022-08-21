using Azure.Storage.Blobs;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Moedim.Microservices.HealthChecks.AzureStorage.Options;

namespace Moedim.Microservices.HealthChecks.AzureStorage;

public class AzureBlobStorageHealthCheck : IHealthCheck
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureBlobStorageHealthCheckOptions _options;

    public AzureBlobStorageHealthCheck(
        AzureBlobStorageHealthCheckOptions options)
    {
        _blobServiceClient = options.Client;
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: BlobServiceClient.GetPropertiesAsync() cannot be used with only the role assignment
            // "Storage Blob Data Contributor," so BlobServiceClient.GetBlobContainersAsync() is used instead to probe service health.
            // However, BlobContainerClient.GetPropertiesAsync() does have sufficient permissions.
            await _blobServiceClient
                            .GetBlobContainersAsync(cancellationToken: cancellationToken)
                            .AsPages(pageSizeHint: 1)
                            .GetAsyncEnumerator(cancellationToken)
                            .MoveNextAsync();

            if (!string.IsNullOrEmpty(_options.ContainerName))
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
                await containerClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            }

            return HealthCheckResult.Healthy($"{_options.ContainerName} is reachable");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
