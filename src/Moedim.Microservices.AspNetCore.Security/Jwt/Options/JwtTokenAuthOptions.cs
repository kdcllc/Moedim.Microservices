using System.Text.Json.Serialization;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Options;

public class JwtTokenAuthOptions
{
    [JsonPropertyName("secret")]
    public string Secret { get; set; } = string.Empty;

    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;

    [JsonPropertyName("audience")]
    public string Audience { get; set; } = string.Empty;

    [JsonPropertyName("accessExpiration")]
    public int AccessExpiration { get; set; } = 30;

    [JsonPropertyName("refreshExpiration")]
    public int RefreshExpiration { get; set; } = 180;

    [JsonPropertyName("salt")]
    public string Salt { get; set; } = string.Empty;
}
