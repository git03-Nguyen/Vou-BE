using EventService.DTOs;
using MediatR;
using Shared.Contracts;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.DeleteQuizSet;

public class DeleteQuizSetCommand : IRequest<BaseResponse<object>>
{
   public string QuizSetId { get; set; }
}