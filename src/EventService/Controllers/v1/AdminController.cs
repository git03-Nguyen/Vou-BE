using Asp.Versioning;
using EventService.Features.Commands.AdminCommands.AcceptEvent;
using EventService.Features.Commands.AdminCommands.RefuseEvent;
using EventService.Features.Queries.AdminQueries.GetAllEvents;
using EventService.Features.Queries.AdminQueries.GetVoucherRedemptions;
using EventService.Features.Queries.StatisticsQueries.EventStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Enums;

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
    public async Task<IActionResult> GetAllEvents(
        [FromQuery] string? counterPartId,
        [FromQuery] EventStatus? status,
        [FromQuery] bool? hasVoucherInventory,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var request = new GetAllEventsQuery
        {
            CounterPartId = counterPartId,
            Status = status,
            HasVoucherInventory = hasVoucherInventory,
            Search = search
        };
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

    [HttpGet("VoucherRedemptions")]
    public async Task<IActionResult> GetVoucherRedemptions(
        [FromQuery] string? eventId,
        [FromQuery] string? voucherId,
        [FromQuery] string? counterPartId,
        [FromQuery] string? playerId,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] DateTime? acquiredFrom,
        [FromQuery] DateTime? acquiredTo,
        [FromQuery] DateTime? usedFrom,
        [FromQuery] DateTime? usedTo,
        [FromQuery] DateTime? expiredFrom,
        [FromQuery] DateTime? expiredTo,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var request = new GetVoucherRedemptionsQuery
        {
            EventId = eventId,
            VoucherId = voucherId,
            CounterPartId = counterPartId,
            PlayerId = playerId,
            Search = search,
            Status = status,
            AcquiredFrom = acquiredFrom,
            AcquiredTo = acquiredTo,
            UsedFrom = usedFrom,
            UsedTo = usedTo,
            ExpiredFrom = expiredFrom,
            ExpiredTo = expiredTo,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
}
