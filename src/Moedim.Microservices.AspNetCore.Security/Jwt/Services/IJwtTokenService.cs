using System.Security.Claims;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
