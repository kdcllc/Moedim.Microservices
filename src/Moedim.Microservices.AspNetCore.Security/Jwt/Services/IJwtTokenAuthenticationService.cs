using Moedim.Microservices.AspNetCore.Security.Jwt.Models;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services;

public interface IJwtTokenAuthenticationService<TId> where TId : struct
{
    Task<AuthorizeTokenResponse> GetTokenAsync(AuthorizeTokenRequest request, CancellationToken cancellationToken);

    Task<AuthorizeTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation);

    Task<bool> RevokeAsync(string userName, CancellationToken cancellation);
}
