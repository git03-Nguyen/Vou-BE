using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Response;

namespace AuthServer.Features.Queries.StatisticsQueries.UserStatistics;

public class UserStatisticsHandler : IRequestHandler<UserStatisticsQuery, BaseResponse<UserStatisticsResponseDto>>
{
    private readonly ILogger<UserStatisticsHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    public UserStatisticsHandler(ILogger<UserStatisticsHandler> logger, UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<UserStatisticsResponseDto>> Handle(UserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserStatisticsResponseDto>();
        const string methodName = $"{nameof(UserStatisticsHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);

        try
        {
           var users = await (
                from user in _userManager.Users
                where !user.IsDeleted
                    select user
                ).ToListAsync(cancellationToken);

            var totalUsers = users.Count;
            var totalActiveUsers = users.Count(u => !u.IsBlocked&&!u.IsDeleted);
            var totalPlayers = users.Count(u => u.Role == Constants.PLAYER);
            var totalCounterParts = users.Count(u => u.Role == Constants.COUNTERPART);

            var responseData = new UserStatisticsResponseDto
            {
                TotalUsers = totalUsers,
                TotalActiveUsers = totalActiveUsers,
                TotalPlayers = totalPlayers,
                TotalCounterParts = totalCounterParts 
            };

            response.ToSuccessResponse(responseData);
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}