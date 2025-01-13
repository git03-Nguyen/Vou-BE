using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.PlayerCommands.ReadNotifications;

public class ReadNotificationsCommand : IRequest<BaseResponse>
{
    public bool IsReadAll { get; set; }
    public List<string> NotificationIds { get; set; }
}