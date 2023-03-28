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

public class ApiKeyHeaderAuthenticationHandler<T> : AuthenticationHandler<ApiKeyHeaderAuthenticationOptions>
    where T : struct
{
    private readonly IUserStoreProvider<T> _userStore;

    public ApiKeyHeaderAuthenticationHandler(
        IOptionsMonitor<ApiKeyHeaderAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserStoreProvider<T> userStore) : base(options, logger, encoder, clock)
    {
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var apiKeyUser = await _userStore.GetByApiKeyAsync(providedApiKey);

        if (apiKeyUser != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, apiKeyUser.Email)
            };

            claims.AddRange(apiKeyUser.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

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

    private Task WriteResponseAsync<TDetail>(TDetail detail) where TDetail : ProblemDetails
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
