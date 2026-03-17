using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.RedeemVoucher;

public class RedeemVoucherCommand : IRequest<BaseResponse<UseVoucherDto>>
{
    public string VoucherToPlayerId { get; set; }
}
