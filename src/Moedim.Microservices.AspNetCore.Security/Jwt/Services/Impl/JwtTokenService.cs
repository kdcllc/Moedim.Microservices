using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Moedim.Microservices.AspNetCore.Security.Jwt.Options;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services.Impl;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtTokenAuthOptions _jwtTokenOptions;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
           IOptionsSnapshot<JwtTokenAuthOptions> jwtTokenOptions,
           ILogger<JwtTokenService> logger)
    {
        _jwtTokenOptions = jwtTokenOptions.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _jwtTokenOptions.Issuer,
                _jwtTokenOptions.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtTokenOptions.AccessExpiration),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Access Token Generation failed");
            throw;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtTokenOptions.Secret)),
            ValidIssuer = _jwtTokenOptions.Issuer,
            ValidAudience = _jwtTokenOptions.Audience,

            // Ensure the token hasn't expired:
            RequireExpirationTime = true,

            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            ClockSkew = TimeSpan.FromMinutes(2), // TimeSpan.Zero,

            RoleClaimType = ClaimTypes.Role,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        return securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
            ? throw new SecurityTokenException("Invalid token")
            : principal;
    }
}
