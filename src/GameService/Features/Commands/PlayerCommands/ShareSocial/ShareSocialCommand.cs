using GameService.DTOs;
using MediatR;
using Shared.Response;

namespace GameService.Features.Commands.PlayerCommands.ShareSocial;

public class ShareSocialCommand : IRequest<BaseResponse<PlayerShakeDto>>
{
    public string EventId { get; set; }
}