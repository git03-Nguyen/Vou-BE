using FluentValidation;

namespace EventService.Features.Commands.AdminCommands.AcceptEvent;

public class AcceptEventValidator : AbstractValidator<AcceptEventCommand>
{
    public AcceptEventValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required")
            .MaximumLength(100)
            .WithMessage("EventId must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9-]*$")
            .WithMessage("EventId must be a valid GUID");
    }
}