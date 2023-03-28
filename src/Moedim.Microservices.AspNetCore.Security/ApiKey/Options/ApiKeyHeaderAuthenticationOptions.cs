using Microsoft.AspNetCore.Authentication;

namespace Moedim.Microservices.AspNetCore.Security.ApiKey.Options;

public class ApiKeyHeaderAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKeyHeaderToken";

    public string AuthenticationType { get; set; } = DefaultScheme;

    public string Scheme => AuthenticationType;

    public string HeaderName { get; set; } = "apiKey";
}
