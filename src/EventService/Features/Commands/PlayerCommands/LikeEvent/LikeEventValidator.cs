using FluentValidation;

namespace EventService.Features.Commands.PlayerCommands.LikeEvent;

public class LikeEventValidator : AbstractValidator<LikeEventCommand>
{
    public LikeEventValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required");

    }
}