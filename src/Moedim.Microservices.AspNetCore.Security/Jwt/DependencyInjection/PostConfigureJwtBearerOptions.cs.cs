using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Moedim.Microservices.AspNetCore.Security.Jwt.Options;

namespace Microsoft.AspNetCore.Authentication.JwtBearer;

public class PostConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public PostConfigureJwtBearerOptions(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void PostConfigure(string name, JwtBearerOptions options)
    {
        if (_configuration != null)
        {
            var tokenOptions = _configuration.GetSection(nameof(JwtTokenAuthOptions)).Get<JwtTokenAuthOptions>();

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,

                ValidateIssuer = true,
                ValidateAudience = true,

                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions!.Secret)),
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience,

                // Ensure the token hasn't expired:
                RequireExpirationTime = true,

                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.FromMinutes(2), // TimeSpan.Zero,

                RoleClaimType = ClaimTypes.Role,
            };
        }
    }
}
