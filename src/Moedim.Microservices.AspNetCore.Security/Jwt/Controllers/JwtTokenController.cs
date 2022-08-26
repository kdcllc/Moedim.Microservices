using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Moedim.Microservices.AspNetCore.Security.Jwt.Models;
using Moedim.Microservices.AspNetCore.Security.Jwt.Services;

namespace Moedim.Microservices.AspNetCore.Security.Jwt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtTokenController : ControllerBase
{
    private readonly IJwtTokenAuthenticationService<int> _authService;

    public JwtTokenController(IJwtTokenAuthenticationService<int> userService)
    {
        _authService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("/token")]
    public async Task<IActionResult> RequestToken(
        [FromBody] AuthorizeTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest("Invalid Request");
    }

    [HttpPost]
    [Route("/refresh")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest("Invalid client request");
    }

    [HttpPost]
    [Authorize]
    [Route("/revoke")]
    public async Task<IActionResult> Revoke(CancellationToken cancellationToken)
    {
        var username = User.Identity.Name;

        if (await _authService.RevokeAsync(username, cancellationToken))
        {
            return NoContent();
        }

        return BadRequest();
    }
}
