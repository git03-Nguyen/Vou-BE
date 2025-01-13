using Asp.Versioning;
using GameService.Features.Commands.PlayerCommands;
using GameService.Features.Commands.PlayerCommands.CompleteShake;
using GameService.Features.Commands.PlayerCommands.SendTicketToFriend;
using GameService.Features.Commands.PlayerCommands.ShareSocial;
using GameService.Features.Queries.PlayerQueries.GetTicketEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace GameService.Controllers;

[Authorize(Policy = Constants.PLAYER)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    #region Shake game
    
    [HttpPost("SendTicket")]
    public async Task<IActionResult> SendTicket([FromBody] SendTicketCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("GetOwnTickets/{eventId}")]
    public async Task<IActionResult> GetTickets([FromRoute] string eventId, CancellationToken cancellationToken)
    {
        GetTicketEventQuery request = new GetTicketEventQuery(eventId);
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("CompleteShake")]
    public async Task<IActionResult> CompleteShake([FromBody] CompleteShakeCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("ShareSocial")]
    public async Task<IActionResult> ShareSocial([FromBody] ShareSocialCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    #endregion
}
