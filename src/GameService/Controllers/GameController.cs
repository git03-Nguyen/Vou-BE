using Asp.Versioning;
using GameService.Features.Queries.GameQueries.GetAllGames;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("GetGames")]
    public async Task<IActionResult> GetAllGames(CancellationToken cancellationToken)
    {
        var request = new GetAllGamesQuery();
        var response = await _mediator.Send(request, cancellationToken);
        return response.ToObjectResult();
    }
}
