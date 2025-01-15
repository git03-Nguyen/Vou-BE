using Asp.Versioning;
using EventService.Features.Commands.PlayerCommands.BuyVoucher;
using EventService.Features.Commands.PlayerCommands.LikeEvent;
using EventService.Features.Commands.PlayerCommands.ReadNotifications;
using EventService.Features.Commands.PlayerCommands.UseVoucher;
using EventService.Features.Queries.CounterPartQueries.GetAllCounterParts;
using EventService.Features.Queries.PlayerQueries.GetAllEvents;
using EventService.Features.Queries.PlayerQueries.GetFavoriteEvents;
using EventService.Features.Queries.PlayerQueries.GetNotifications;
using EventService.Features.Queries.PlayerQueries.GetOwnVouchers;
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

    #region CounterParts

    [HttpGet("GetCounterParts")]
    public async Task<IActionResult> GetCounterParts(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllCounterPartsQuery(), cancellationToken);
        return response.ToObjectResult();
    }

    #endregion

    #region Events

    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllEventsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpGet("GetFavoriteEvents")]
    public async Task<IActionResult> GetFavoriteEvents(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetFavoriteEventsQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("LikeEvent")]
    public async Task<IActionResult> LikeEvent([FromBody] LikeEventCommand request,CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
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
    public async Task<IActionResult> ReadNotifications([FromBody] ReadNotificationsCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion

    #region Vouchers

    [HttpGet("GetOwnVouchers")]
    public async Task<IActionResult> GetOwnVouchers(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnVouchersQuery(), cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("BuyVoucher")]
    public async Task<IActionResult> BuyVoucher([FromBody] BuyVoucherCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
    
    [HttpPost("UseVoucher/{voucherToPlayerId}")]
    public async Task<IActionResult> UseVoucher([FromRoute] string voucherToPlayerId, CancellationToken cancellationToken)
    {
        var request = new UseVoucherCommand { VoucherToPlayerId = voucherToPlayerId };
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion
}