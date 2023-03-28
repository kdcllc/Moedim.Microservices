using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;

using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Services.Impl;

public class RoleClaimsTransformation : IClaimsTransformation
{
    private readonly IUserStoreProvider<int> _userStore;

    public RoleClaimsTransformation(IUserStoreProvider<int> userStore)
    {
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();

        if (!principal.HasClaim(claim => claim.Type == ClaimTypes.Role))
        {
            var username = principal?.Identities?.First()?.Name;

            if (username != null)
            {
                var user = await _userStore.GetByUserNameAsync(username);
                foreach (var claim in user.Roles.Select(role => new Claim(ClaimTypes.Role, role)))
                {
                    claimsIdentity.AddClaim(claim);
                }
            }
        }

        principal?.AddIdentity(claimsIdentity);

        return principal!;
    }
}
