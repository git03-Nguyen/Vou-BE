using GameService.DTOs;
using MediatR;
using Shared.Response;

namespace GameService.Features.Queries.GameQueries.GetAllGames;

public class GetAllGamesQuery : IRequest<BaseResponse<GetAllGamesResponse>>
{
}

public class GetAllGamesResponse
{
    public List<GameDetailDto> Games { get; set; }
}