using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Queries.PlayerQueries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, BaseResponse<GetNotificationsResponse>>
{
    private readonly ILogger<GetNotificationsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public GetNotificationsHandler(ILogger<GetNotificationsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse<GetNotificationsResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(GetNotificationsHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<GetNotificationsResponse>();
        
        try
        {
            var notifications = await
            (
                from notification in _unitOfWork.Notifications.GetAll()
                where !notification.IsDeleted && notification.PlayerId == userId
                select new NotificationDto
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Content = notification.Content,
                    CreatedDate = notification.CreatedDate ?? DateTime.Now,
                    IsRead = notification.IsRead
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            var responseData = new GetNotificationsResponse { Notifications = notifications };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }
        
        return response;
    }
}