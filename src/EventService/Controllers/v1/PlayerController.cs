using Asp.Versioning;
using EventService.Features.Queries.PlayerQueries.GetFavoriteEvents;
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
    
    [HttpGet("GetFavoriteEvents")]
    public async Task<IActionResult> GetFavoriteEvents(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetFavoriteEventsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
}