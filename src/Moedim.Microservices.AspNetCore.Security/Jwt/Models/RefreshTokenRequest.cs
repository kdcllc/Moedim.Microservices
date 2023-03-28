using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Models;

public class RefreshTokenRequest
{
    [Required]
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
