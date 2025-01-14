using EventService.DTOs;
using EventService.DTOs.InputDtos;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.CreateEvent;

public class CreateEventCommand : IRequest<BaseResponse<FullEventDto>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ShakeSessionInputDto? ShakeSession { get; set; }
    public QuizSessionInputDto[]? QuizSessions { get; set; }
}