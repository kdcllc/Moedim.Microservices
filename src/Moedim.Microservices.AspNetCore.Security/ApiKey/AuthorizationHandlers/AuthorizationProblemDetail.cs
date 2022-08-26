using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace Moedim.Microservices.AspNetCore.Security.ApiKey.AuthorizationHandlers;

internal class AuthorizationProblemDetail
{
    public static string ContentType = "application/problem+json";

    public static JsonSerializerOptions DefaultJsonSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string GetProblemDetails<T>(T problem) where T : ProblemDetails
    {
        return JsonSerializer.Serialize(problem, DefaultJsonSerializerOptions);
    }
}
