using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.CreateVoucher;

public class CreateVoucherCommand : IRequest<BaseResponse<VoucherDto>>
{
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
}