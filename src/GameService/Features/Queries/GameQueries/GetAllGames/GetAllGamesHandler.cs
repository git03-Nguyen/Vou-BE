using GameService.DTOs;
using GameService.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace GameService.Features.Queries.GameQueries.GetAllGames;

public class GetAllGamesHandler : IRequestHandler<GetAllGamesQuery, BaseResponse<GetAllGamesResponse>>
{
    private readonly ILogger<GetAllGamesHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetAllGamesHandler(ILogger<GetAllGamesHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<GetAllGamesResponse>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<GetAllGamesResponse>();
        const string methodName = $"{nameof(GetAllGamesHandler)}.{nameof(Handle)} =>";
        _logger.LogInformation(methodName);

        try
        {
            var games = await _unitOfWork.Games
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .Select(x => new GameDetailDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Author = x.Author,
                    Type = x.Type,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync(cancellationToken);

            var responseData = new GetAllGamesResponse { Games = games };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, methodName);
            response.ToInternalErrorResponse();
        }

        return response;
    }
}