using Asp.Versioning;
using EventService.Features.Commands.AdminCommands.AcceptEvent;
using EventService.Features.Commands.AdminCommands.RefuseEvent;
using EventService.Features.Queries.AdminQueries.GetAllEvents;
using EventService.Features.Queries.StatisticsQueries.EventStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace EventService.Controllers.v1;

[Authorize(Policy = Constants.ADMIN)]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
    {
        var request = new GetAllEventsQuery();
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("AcceptEvent/{eventId}")]
    public async Task<IActionResult> AcceptEvent(string eventId, CancellationToken cancellationToken)
    {
        var request = new AcceptEventCommand(eventId);
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("RefuseEvent/{eventId}")]
    public async Task<IActionResult> RefuseEvent(string eventId, CancellationToken cancellationToken)
    {
        var request = new RefuseEventCommand(eventId);
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("EventStatistics")]
    public async Task<IActionResult> GetEventStatistics(CancellationToken cancellationToken)
    {
        var request = new EventStatisticsQuery();
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
}