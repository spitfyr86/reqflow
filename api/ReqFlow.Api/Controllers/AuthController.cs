using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReqFlow.Application;

namespace ReqFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IUserRepository userRepository, JwtTokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("demo-users")]
    public async Task<ActionResult<IReadOnlyList<CurrentUserDto>>> ListDemoUsers(CancellationToken cancellationToken) =>
        Ok((await userRepository.ListActiveAsync(cancellationToken))
            .Select(user => new CurrentUserDto(user.Id, user.Email, user.DisplayName, user.Role.ToString()))
            .ToList());

    [AllowAnonymous]
    [HttpPost("demo-login")]
    public async Task<ActionResult<LoginResponseDto>> DemoLogin(DemoLoginDto dto, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(dto.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new ForbiddenException("The selected demo user is not active.");
        }

        return Ok(tokenService.Create(user));
    }
}
