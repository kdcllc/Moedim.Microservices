namespace Moedim.Microservices.Options
{
    public class AzureVaultOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string HealthCheckSecret { get; set; } = nameof(HealthCheckSecret);
    }
}
