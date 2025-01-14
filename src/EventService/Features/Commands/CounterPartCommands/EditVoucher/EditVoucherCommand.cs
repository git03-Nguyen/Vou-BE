using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.EditVoucher;

public class EditVoucherCommand : IRequest<BaseResponse<VoucherDto>>
{
    public string Id { get; set; }
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public int? Value { get; set; }
}