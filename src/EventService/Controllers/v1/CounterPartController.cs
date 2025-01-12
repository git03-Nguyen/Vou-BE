using Asp.Versioning;
using EventService.Features.Commands.CounterPartCommands.AddQuizSession;
using EventService.Features.Commands.CounterPartCommands.CreateEvent;
using EventService.Features.Commands.CounterPartCommands.CreateQuizSet;
using EventService.Features.Commands.CounterPartCommands.CreateVoucher;
using EventService.Features.Queries.CounterPartQueries.GetOwnEvent;
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

    #region Event
    
    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnEventsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("GetEvent/{eventId}")]
    public async Task<IActionResult> GetEvents([FromRoute] string eventId, CancellationToken cancellationToken)
    {
        var request = new GetOwnEventQuery { EventId = eventId };
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("CreateEvent")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion

    #region Voucher

    [HttpGet("GetVouchers")]
    public async Task<IActionResult> GetOwnVouchers(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnVouchersQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("CreateVoucher")]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion

    #region QuizSet

    [HttpGet("GetQuizSets")]
    public async Task<IActionResult> GetQuizSets(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnQuizSetsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("CreateQuizSet")]
    public async Task<IActionResult> CreateQuizSet([FromBody] CreateQuizSetCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion

    #region Game

    [HttpPost("AddQuizSession")]
    public async Task<IActionResult> AddQuizSession([FromBody] AddQuizSessionCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion
    
}