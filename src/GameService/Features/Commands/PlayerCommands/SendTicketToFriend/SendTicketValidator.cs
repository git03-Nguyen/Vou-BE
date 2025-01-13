using FluentValidation;

namespace GameService.Features.Commands.PlayerCommands.SendTicketToFriend;

public class SendTicketValidator : AbstractValidator<SendTicketCommand>
{
    public SendTicketValidator()
    {
        RuleFor(x => x.EventId)
        .NotNull()
        .NotEmpty()
        .WithMessage("EventId is required")
        .MaximumLength(100)
        .WithMessage("EventId is too long");

        RuleFor(x => x.UserNameOrEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage("UserNameOrEmail is required")
            .MaximumLength(100);
    }
}