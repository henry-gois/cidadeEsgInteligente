using Esg.Api.Services;
using Esg.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Esg.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthenticationController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var token = await authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<TokenResponse>.Ok(token, "Authentication completed successfully"));
    }
}
