using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services;

public interface IUserService<TId> where TId : struct
{
    /// <summary>
    /// Validates Existing User of the Api.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsValidUserAsync(string username, string password, CancellationToken cancellationToken);

    /// <summary>
    /// Update refresh token.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="refreshToken"></param>
    /// <param name="refreshTokenExpiryTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> UpdateRefreshTokenAsync(
        string userName,
        string refreshToken,
        DateTimeOffset refreshTokenExpiryTime,
        CancellationToken cancellationToken);

    /// <summary>
    /// Validate existing refresh token.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ValidateRefreshTokenAsync(
        string userName,
        string refreshToken,
        CancellationToken cancellationToken);

    Task<User<TId>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
}
