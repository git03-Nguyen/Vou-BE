using Asp.Versioning;
using AuthServer.Features.Commands.PlayerCommands.RegisterPlayer;
using AuthServer.Features.Commands.UserCommands.ChangePassword;
using AuthServer.Features.Commands.UserCommands.ConfirmActivateOtp;
using AuthServer.Features.Commands.UserCommands.UpdateOwnUserProfile;
using AuthServer.Features.Commands.UserCommands.UserLogin;
using AuthServer.Features.Queries.UserQueries.GetOwnProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace AuthServer.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
[Authorize(Policy = Constants.PLAYER)]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterPlayerCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginCommand request, CancellationToken cancellationToken)
    {
        request.Role = Constants.PLAYER;
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("Profile")]
    public async Task<IActionResult> GetOwnProfile(CancellationToken cancellationToken)
    {
        var request = new GetOwnProfileQuery();
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    // [HttpPost("ForgotPassword")]
    // public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    // {
    //     var response = await _mediator.Send(command, cancellationToken);
    //     return response.ToObjectResult();
    // }
    
    [HttpPatch("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateOwnUserProfileCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    [AllowAnonymous]
    [HttpPost("ActivateAccount")]
    public async Task<IActionResult> ActivateAccount([FromBody] ConfirmActivateOtpCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
}