using Asp.Versioning;
using Fundo.Loan.Application.Users.Common;
using Fundo.Loan.Application.Users.Dtos;
using Fundo.Loan.Application.Users.GetAllUsers;
using Fundo.Loan.Application.Users.GetProfile;
using Fundo.Loan.Application.Users.LoginUser;
using Fundo.Loan.Application.Users.LogoutUser;
using Fundo.Loan.Application.Users.RegisterUser;
using Fundo.Loan.Application.Users.UpdateProfile;
using Fundo.Loan.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Fundo.Loan.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] // Default authorization for the controller
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        AuthResponse result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        AuthResponse result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutUserCommand());
        return Ok("Logged out successfully.");
    }

    [HttpGet("all")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(List<UserDto>), 200)]
    public async Task<IActionResult> GetAllUsers()
    {
        List<UserDto> users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), 200)]
    public async Task<IActionResult> GetProfile()
    {
        UserProfileDto profile = await _mediator.Send(new GetProfileQuery());
        return Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
    {
        // Ensure we don't accidentally overwrite someone else if the payload is malicious
        command.AppUserId = null;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(UserProfileDto), 200)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        UserProfileDto profile = await _mediator.Send(new GetProfileQuery { AppUserId = id });
        return Ok(profile);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AdminUpdateUser(Guid id, UpdateProfileCommand command)
    {
        command.AppUserId = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
