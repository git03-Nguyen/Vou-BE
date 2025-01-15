using EventService.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace EventService.Features.Queries.PlayerQueries.GetNotifications;

public class GetNotificationsQuery : IRequest<BaseResponse<GetNotificationsResponse>>
{
}

public class GetNotificationsResponse
{
    public List<NotificationDto> Notifications { get; set; }
}