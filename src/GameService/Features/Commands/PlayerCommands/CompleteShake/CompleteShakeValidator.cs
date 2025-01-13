using FluentValidation;

namespace GameService.Features.Commands.PlayerCommands.CompleteShake;

public class CompleteShakeValidator : AbstractValidator<CompleteShakeCommand>
{
    public CompleteShakeValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("EventId is required");
    }
}