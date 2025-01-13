using Asp.Versioning;
using GameService.Features.Commands.PlayerCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace GameService.Controllers;

[Authorize(Policy = Constants.PLAYER)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/{controller}")]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    #region Shake ticket
    
    [HttpPost("SendTicket")]
    public async Task<IActionResult> SendTicket([FromBody] SendTicketCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    #endregion
}