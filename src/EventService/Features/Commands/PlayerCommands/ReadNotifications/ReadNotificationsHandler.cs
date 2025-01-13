using EventService.Data.Models;
using EventService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace EventService.Features.Commands.PlayerCommands.ReadNotifications;

public class ReadNotificationsHandler : IRequestHandler<ReadNotificationsCommand, BaseResponse>
{
    private readonly ILogger<ReadNotificationsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    public ReadNotificationsHandler(ILogger<ReadNotificationsHandler> logger, IUnitOfWork unitOfWork, ICustomHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponse> Handle(ReadNotificationsCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(ReadNotificationsHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse();

        try
        {
            List<Notification> notifications;
            if (request.IsReadAll)
            {
                notifications = await _unitOfWork.Notifications
                    .Where(x => 
                        x.PlayerId == userId 
                        && !x.IsDeleted 
                        && !x.IsRead)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                notifications = await _unitOfWork.Notifications
                    .Where(x => 
                        x.PlayerId == userId 
                        && !x.IsRead 
                        && request.NotificationIds.Contains(x.Id) 
                        && !x.IsDeleted)
                    .ToListAsync(cancellationToken);
            }
            
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            response.ToSuccessResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}