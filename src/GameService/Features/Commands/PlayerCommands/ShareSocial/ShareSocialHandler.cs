using GameService.DTOs;
using GameService.Helpers;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using Shared.Services.HttpContextAccessor;

namespace GameService.Features.Commands.PlayerCommands.ShareSocial;

public class ShareSocialHandler : IRequestHandler<ShareSocialCommand, BaseResponse<PlayerShakeDto>>
{
    private readonly ILogger<ShareSocialHandler> _logger;
    private readonly ICustomHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    public ShareSocialHandler(ILogger<ShareSocialHandler> logger, ICustomHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<PlayerShakeDto>> Handle(ShareSocialCommand request, CancellationToken cancellationToken)
    {
        var userId = _contextAccessor.GetCurrentUserId();
        var methodName = $"{nameof(ShareSocialHandler)}.{nameof(Handle)} UserId: {userId} =>";
        _logger.LogInformation(methodName);
        var response = new BaseResponse<PlayerShakeDto>();

        try
        {
            var player = await _unitOfWork.PlayerShakeSessions
                .Where(x =>
                    !x.IsDeleted
                    && x.EventId == request.EventId
                    && x.PlayerId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (player is null)
            {
                response.ToBadRequestResponse("Player not found");
                return response;
            }

            // Check if player has shared social in last week
            if (player.LastShareTime is null 
                || player.LastShareTime.Value < TimeHelpers.GetNearestLastMonday())
            {
                player.Tickets++;
            }
            player.LastShareTime = DateTime.Now;

            _unitOfWork.PlayerShakeSessions.Update(player);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseData = new PlayerShakeDto
            {
                Id = userId,
                Tickets = player.Tickets,
                Diamonds = player.Diamond
            };

            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, methodName);
            response.ToInternalErrorResponse();
        }

        return response;
    }
}