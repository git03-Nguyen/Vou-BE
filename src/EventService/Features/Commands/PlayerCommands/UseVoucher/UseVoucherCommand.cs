using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.PlayerCommands.UseVoucher;

public class UseVoucherCommand : IRequest<BaseResponse<UseVoucherDto>>
{
    public string VoucherToPlayerId { get; set; }
}