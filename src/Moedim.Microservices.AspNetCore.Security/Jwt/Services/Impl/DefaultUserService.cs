using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services.Impl;

public class DefaultUserService<TId> : IUserService<TId> where TId : struct
{
    private readonly IUserStoreProvider<TId> _userStore;

    public DefaultUserService(IUserStoreProvider<TId> userStore)
    {
        _userStore = userStore;
    }

    public Task<User<TId>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _userStore.GetByUserNameAsync(userName, cancellationToken);
    }

    public async Task<bool> IsValidUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _userStore.GetByUserNameAsync(username, cancellationToken);

        if (user == null)
        {
            return false;
        }

        return user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)
            && user.Password.Equals(password, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> UpdateRefreshTokenAsync(
        string username,
        string refreshToken,
        DateTimeOffset refreshTokenExpiryTime,
        CancellationToken cancellationToken)
    {
        var user = await _userStore.GetByUserNameAsync(username, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

        await _userStore.SaveAsync(user, cancellationToken);

        return true;
    }

    public async Task<bool> ValidateRefreshTokenAsync(
        string username,
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var user = await _userStore.GetByUserNameAsync(username, cancellationToken);
        if (user == null)
        {
            return false;
        }

        if (user.RefreshToken.Equals(refreshToken, StringComparison.OrdinalIgnoreCase)
            || user.RefreshTokenExpiryTime >= DateTimeOffset.Now)
        {
            return true;
        }

        return false;
    }
}
