using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moedim.Microservices.AspNetCore.Security.ApiKey.AuthorizationHandlers;
using Moedim.Microservices.AspNetCore.Security.ApiKey.Options;
using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Microsoft.AspNetCore.Authentication;

public class ApiKeyQueryStringAuthenticationHandler<T> : AuthenticationHandler<ApiKeyStringQueryAuthenticationOptions>
                where T : struct
{
    private readonly IUserStoreProvider<T> _userStore;

    public ApiKeyQueryStringAuthenticationHandler(
        IOptionsMonitor<ApiKeyStringQueryAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserStoreProvider<T> userStore) : base(options, logger, encoder, clock)
    {
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Query.TryGetValue(Options.QueryStringName, out var apiKeyQueryValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyQueryValues.FirstOrDefault();

        if (apiKeyQueryValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var existingApiKey = await _userStore.GetByApiKeyAsync(providedApiKey);

        if (existingApiKey != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingApiKey.Email)
            };

            claims.AddRange(existingApiKey.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.NoResult();
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        await WriteResponseAsync(new UnauthorizedProblemDetails());
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        await WriteResponseAsync(new ForbiddenProblemDetails());
    }

    private Task WriteResponseAsync<T>(T detail) where T : ProblemDetails
    {
        if (!Response.HasStarted)
        {
            Response.StatusCode = detail.Status.GetValueOrDefault();
            Response.ContentType = AuthorizationProblemDetail.ContentType;

            return Response.WriteAsync(AuthorizationProblemDetail.GetProblemDetails(detail));
        }

        return Task.CompletedTask;
    }
}
