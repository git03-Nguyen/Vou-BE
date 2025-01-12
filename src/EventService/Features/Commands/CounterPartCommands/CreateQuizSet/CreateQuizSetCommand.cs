using EventService.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.CreateQuizSet;

public class CreateQuizSetCommand : IRequest<BaseResponse<QuizSetDto>>
{
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public List<Quiz> Quizes { get; set; }
}