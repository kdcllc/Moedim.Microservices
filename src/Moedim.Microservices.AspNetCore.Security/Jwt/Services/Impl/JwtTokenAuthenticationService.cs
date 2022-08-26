using System.Security.Claims;

using Microsoft.Extensions.Options;

using Moedim.Microservices.AspNetCore.Security.Jwt.Models;
using Moedim.Microservices.AspNetCore.Security.Jwt.Options;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services.Impl;

public class JwtTokenAuthenticationService<TId> : IJwtTokenAuthenticationService<TId> where TId : struct
{
    private readonly IUserService<TId> _userService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtTokenAuthOptions _jwtTokenOptions;

    public JwtTokenAuthenticationService(
            IUserService<TId> userService,
            IJwtTokenService jwtTokenService,
            IOptionsSnapshot<JwtTokenAuthOptions> jwtTokenOptions)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));

        _jwtTokenOptions = jwtTokenOptions.Value;
    }

    public async Task<AuthorizeTokenResponse> GetTokenAsync(AuthorizeTokenRequest request, CancellationToken cancellationToken)
    {
        var result = new AuthorizeTokenResponse();

        if (!await _userService.IsValidUserAsync(request.UserName, request.Password, cancellationToken))
        {
            return result;
        }

        var user = await _userService.GetByUserNameAsync(request.UserName, cancellationToken);

        var claims = new[]
        {
                new Claim(ClaimTypes.Name, request.UserName)
        }.ToList();

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        try
        {
            result.Success = true;
            result.AccessToken = _jwtTokenService.GenerateAccessToken(claims);
            result.ExpiresIn = DateTimeOffset.UtcNow.AddMinutes(_jwtTokenOptions.AccessExpiration).Ticks;
            result.RefreshToken = _jwtTokenService.GenerateRefreshToken();

            await _userService.UpdateRefreshTokenAsync(
                request.UserName,
                result.RefreshToken,
                DateTimeOffset.Now.Add(TimeSpan.FromMinutes(_jwtTokenOptions.AccessExpiration)),
                cancellationToken);
        }
        catch
        {
            result.Success = false;
        }

        return result;
    }

    public async Task<AuthorizeTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = new AuthorizeTokenResponse();

        try
        {
            var accessToken = request.AccessToken;
            var refreshToken = request.RefreshToken;

            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken);

            // this is mapped to the Name claim by default
            var username = principal.Identity.Name;

            if (!await _userService.ValidateRefreshTokenAsync(username ?? string.Empty, refreshToken, cancellationToken))
            {
                return result;
            }

            result.Success = true;
            result.AccessToken = _jwtTokenService.GenerateAccessToken(principal.Claims);
            result.ExpiresIn = DateTimeOffset.UtcNow.AddMinutes(_jwtTokenOptions.AccessExpiration).Ticks;
            result.RefreshToken = _jwtTokenService.GenerateRefreshToken();

            await _userService.UpdateRefreshTokenAsync(
                username ?? string.Empty,
                result.RefreshToken,
                DateTime.Now.Add(TimeSpan.FromMinutes(_jwtTokenOptions.RefreshExpiration)),
                cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
        }

        return result;
    }

    public async Task<bool> RevokeAsync(string userName, CancellationToken cancellation)
    {
        return await _userService.UpdateRefreshTokenAsync(userName, string.Empty, default, cancellation);
    }
}
