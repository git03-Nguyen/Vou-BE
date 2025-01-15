using System.Text.Json;
using GameService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Contracts.ServiceInvocations;

namespace GameService.InternalControllers;

[AllowAnonymous]
[ApiController]
[Route("Internal/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public PlayerController(ILogger<PlayerController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    [HttpPost("GetPlayerTicketDiamond")]
    public async Task<IActionResult> GetPlayerTicketDiamond([FromBody] PlayerTicketDiamondRequest request, CancellationToken cancellationToken)
    {
        var methodName = $"{nameof(PlayerController)}.{nameof(GetPlayerTicketDiamond)} Payload = {JsonSerializer.Serialize(request)} =>"; 
        _logger.LogInformation(methodName);
        var response = new PlayerTicketDiamondResponse();

        try
        {
            var player = await _unitOfWork.PlayerShakeSessions
                .Where(x => 
                    x.PlayerId == request.PlayerId
                    && x.EventId == request.EventId)
                .Select(x => new
                {
                    Tickets = x.Tickets,
                    Diamonds = x.Diamond
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (player is null)
            {
                return NotFound(response);
            }
            
            response.Diamonds = player.Diamonds;
            response.Tickets = player.Tickets;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName} Has error: {ex.Message}");
            return StatusCode(500, response);
        }
    }
    
    [HttpPost("UpdatePlayerDiamond")]
    public async Task<IActionResult> UpdatePlayerDiamond([FromBody] UpdatePlayerDiamondRequest request, CancellationToken cancellationToken)
    {
        var methodName = $"{nameof(PlayerController)}.{nameof(UpdatePlayerDiamond)} Payload = {JsonSerializer.Serialize(request)} =>"; 
        _logger.LogInformation(methodName);
        try
        {
            var player = await _unitOfWork.PlayerShakeSessions
                .Where(x => 
                    x.PlayerId == request.PlayerId
                    && x.EventId == request.EventId)
                .FirstOrDefaultAsync(cancellationToken);
            if (player is null)
            {
                return NotFound();
            }

            player.Diamond = (int)request.Diamonds;
            _unitOfWork.PlayerShakeSessions.Update(player);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName} Has error: {ex.Message}");
            return StatusCode(500);
        }
    }
}