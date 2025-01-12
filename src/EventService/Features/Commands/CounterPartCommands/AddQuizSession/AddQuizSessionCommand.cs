using EventService.DTOs;
using MediatR;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.AddQuizSession;

public class AddQuizSessionCommand : IRequest<BaseResponse<FullEventDto>>
{
    public string EventId { get; set; }
    public string VoucherId { get; set; }
    public string QuizSetId { get; set; }
    public int TakeTop { get; set; } = 50; // 50%
    public DateTime StartTime { get; set; }
}