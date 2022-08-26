using System.Text.Json.Serialization;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Models;

public class AuthorizeTokenResponse
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; set; }

    internal bool Success { get; set; }
}
