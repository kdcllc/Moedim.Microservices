using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Models;

public class AuthorizeTokenRequest
{
    [Required]
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
