using GameService.DTOs;
using MediatR;
using Shared.Response;

namespace GameService.Features.Commands.PlayerCommands.CompleteShake;

public class CompleteShakeCommand : IRequest<BaseResponse<ShakeResultDto>>
{
    public string EventId { get; set; }
}