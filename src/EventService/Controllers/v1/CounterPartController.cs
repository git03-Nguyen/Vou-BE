using Asp.Versioning;
using EventService.Features.Queries.CounterPartQueries.GetOwnEvents;
using EventService.Features.Queries.CounterPartQueries.GetOwnQuizSets;
using EventService.Features.Queries.CounterPartQueries.GetOwnVouchers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace EventService.Controllers.v1;

[Authorize(Roles = Constants.COUNTERPART)]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class CounterPartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CounterPartController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnEventsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("GetQuizSets")]
    public async Task<IActionResult> GetQuizSets(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnQuizSetsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("GetVouchers")]
    public async Task<IActionResult> GetOwnVouchers(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnVouchersQuery(), cancellationToken);
        return response.ToObjectResult();
    }
}