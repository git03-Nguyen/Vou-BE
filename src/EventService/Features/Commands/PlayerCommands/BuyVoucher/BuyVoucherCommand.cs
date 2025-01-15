using EventService.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace EventService.Features.Commands.PlayerCommands.BuyVoucher;

public class BuyVoucherCommand : IRequest<BaseResponse<BuyVoucherDto>>
{
    public string EventId { get; set; }
}