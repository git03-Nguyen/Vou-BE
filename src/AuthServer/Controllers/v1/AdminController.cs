using MediatR;
using Asp.Versioning;
using AuthServer.Features.Commands.AdminCommands.BlockUser;
using AuthServer.Features.Commands.AdminCommands.CreateUser;
using AuthServer.Features.Commands.AdminCommands.UnblockUser;
using AuthServer.Features.Commands.AdminCommands.UpdateUser;
using AuthServer.Features.Commands.UserCommands.ChangePassword;
using AuthServer.Features.Commands.UserCommands.UserLogin;
using AuthServer.Features.Queries.AdminQueries.GetAllUsers;
using AuthServer.Features.Queries.UserQueries.GetOwnProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace AuthServer.Controllers.v1;

[Authorize(Roles = Constants.ADMIN)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginCommand request, CancellationToken cancellationToken)
    {
        request.Role = Constants.ADMIN;
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("GetUsers")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var request = new GetAllUsersQuery();
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("BlockUser")]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("UnblockUser")]
    public async Task<IActionResult> UnblockUser([FromBody] UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    // [HttpPost("ResetUserPassword")]
    // public async Task<IActionResult> ResetUserPassword([FromBody] ResetUserPasswordCommand request, CancellationToken cancellationToken)
    // {
    // }
    
    [HttpGet("Profile")]
    public async Task<IActionResult> GetOwnProfile(CancellationToken cancellationToken)
    {
        var request = new GetOwnProfileQuery();
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPatch("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
}
