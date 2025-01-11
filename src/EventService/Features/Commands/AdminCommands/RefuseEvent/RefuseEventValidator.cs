using EventService.Features.Commands.AdminCommands.AcceptEvent;
using FluentValidation;

namespace EventService.Features.Commands.AdminCommands.RefuseEvent;

public class RefuseEventValidator : AbstractValidator<RefuseEventCommand>
{
    public RefuseEventValidator()
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