using FluentValidation;
using MediatR;
using Shared.Common;
using Shared.Response;

namespace EventService.Features.Commands.CounterPartCommands.DeleteQuizSet;

public class DeleteQuizSetValidator : AbstractValidator<DeleteQuizSetCommand>, IRequest<BaseResponse<object>>
{
    public DeleteQuizSetValidator()
    {
        RuleFor(x => x.QuizSetId)
            .NotNull()
            .NotEmpty()
            .WithMessage("QuizSetId is required")
            .MaximumLength(100)
            .WithMessage("QuizSetId is too long");
    }
}