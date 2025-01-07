using Asp.Versioning;
using AuthServer.Common;
using AuthServer.Features.Queries.GetAllUsers;
using AuthServer.Features.Queries.GetOwnProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Admin section

    [Authorize(Roles = Constants.ADMIN)]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var response = await _mediator.Send(query, cancellationToken);
        return response.ToObjectResult();
    }
    

    #endregion

    #region Account owner section

    [Authorize]
    [HttpGet("Profile")]
    public async Task<IActionResult> GetOwnProfile(CancellationToken cancellationToken)
    {
        var query = new GetOwnProfileQuery();
        var response = await _mediator.Send(query, cancellationToken);
        return response.ToObjectResult();
    }

    #endregion
    
    
}
