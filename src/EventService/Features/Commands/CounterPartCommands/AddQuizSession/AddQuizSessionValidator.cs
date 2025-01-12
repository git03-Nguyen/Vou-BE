using FluentValidation;

namespace EventService.Features.Commands.CounterPartCommands.AddQuizSession;

public class AddQuizSessionValidator : AbstractValidator<AddQuizSessionCommand>
{
    public AddQuizSessionValidator()
    {
        RuleFor(x => x.EventId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("EventId is required and must not exceed 50 characters");
        
        RuleFor(x => x.VoucherId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("VoucherId is required and must not exceed 50 characters");
        
        RuleFor(x => x.QuizSetId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("QuizSetId is required and must not exceed 50 characters");
        
        RuleFor(x => x.TakeTop)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("TakeTop is required and must be greater than 0");
        
        RuleFor(x => x.StartTime)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .GreaterThan(DateTime.Now + TimeSpan.FromMinutes(1))
            .WithMessage("StartTime is required and must be greater than the current time");
    }
}