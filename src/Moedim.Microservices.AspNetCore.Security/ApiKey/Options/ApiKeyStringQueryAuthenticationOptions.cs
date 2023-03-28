using Microsoft.AspNetCore.Authentication;

namespace Moedim.Microservices.AspNetCore.Security.ApiKey.Options;

public class ApiKeyStringQueryAuthenticationOptions : AuthenticationSchemeOptions
{
    public static readonly string DefaultScheme = "ApiKeyQueryStringToken";

    public string AuthenticationType { get; set; } = DefaultScheme;

    public string Scheme => AuthenticationType;

    public string QueryStringName { get; set; } = "token";
}
