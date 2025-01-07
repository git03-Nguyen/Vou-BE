using Asp.Versioning;
using AuthServer.Features.Commands.LoginForUser;
using AuthServer.Features.Commands.RegisterForPlayer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterForPlayerCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginForUserCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return response.ToObjectResult();
    }
}
