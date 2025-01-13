using Asp.Versioning;
using EventService.Features.Commands.PlayerCommands.ReadNotifications;
using EventService.Features.Queries.PlayerQueries.GetFavoriteEvents;
using EventService.Features.Queries.PlayerQueries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace EventService.Controllers.v1;

[Authorize(Policy = Constants.PLAYER)]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Favorite events

    [HttpGet("GetFavoriteEvents")]
    public async Task<IActionResult> GetFavoriteEvents(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetFavoriteEventsQuery(), cancellationToken);
        return response.ToObjectResult();
    }

    #endregion

    #region Notifications

    [HttpGet("GetNotifications")]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetNotificationsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("ReadNotifications")]
    public async Task<IActionResult> ReadNotifications([FromBody] ReadNotificationsCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion
}