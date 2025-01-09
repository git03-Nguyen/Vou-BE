using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/{apiVersion:apiVersion}/[controller]")]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;
    public EventController(IMediator mediator)
    {
        _mediator = mediator;
    }
}