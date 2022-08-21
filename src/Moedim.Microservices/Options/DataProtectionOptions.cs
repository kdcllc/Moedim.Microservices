using System.ComponentModel.DataAnnotations;

namespace Moedim.Microservices.Options;

public class DataProtectionOptions
{
    [Url]
    [Required]
    public string AzureBlobStorageUrl { get; set; } = string.Empty;

    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string ContainerName { get; set; } = string.Empty;
}
