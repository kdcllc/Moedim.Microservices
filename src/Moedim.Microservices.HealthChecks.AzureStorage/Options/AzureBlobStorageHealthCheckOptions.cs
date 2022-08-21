using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace Moedim.Microservices.HealthChecks.AzureStorage.Options;

/// <summary>
/// Represents a collection of settings that configure an
/// <see cref="AzureBlobStorageHealthCheck">Azure Storage Blob Service health check</see>.
/// </summary>
public sealed class AzureBlobStorageHealthCheckOptions
{
    public string BlobServiceUri { get; set; } = string.Empty;

    public TokenCredential Credential { get; set; } = new DefaultAzureCredential();

    public BlobClientOptions? ClientOptions { get; set; }

    public BlobServiceClient Client => new BlobServiceClient(new Uri(BlobServiceUri), Credential, ClientOptions);

    /// <summary>
    /// Gets or sets the name of the Azure Storage container whose health should be checked.
    /// </summary>
    /// <remarks>
    /// If the value is <see langword="null"/>, then no health check is performed for a specific container.
    /// </remarks>
    /// <value>An optional Azure Storage container name.</value>
    public string? ContainerName { get; set; }
}
