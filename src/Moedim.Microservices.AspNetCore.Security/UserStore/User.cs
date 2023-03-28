namespace Moedim.Microservices.AspNetCore.Security.UserStore;

public class User<TId> where TId : notnull
{
    public TId Id { get; set; } = default!;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new List<string>();

    public string RefreshToken { get; set; } = string.Empty;

    public DateTimeOffset RefreshTokenExpiryTime { get; set; }
}
