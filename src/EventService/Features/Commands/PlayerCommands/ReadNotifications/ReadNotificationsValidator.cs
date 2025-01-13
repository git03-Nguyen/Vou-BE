using FluentValidation;

namespace EventService.Features.Commands.PlayerCommands.ReadNotifications;

public class ReadNotificationsValidator : AbstractValidator<ReadNotificationsCommand>
{
    public ReadNotificationsValidator()
    {
        When(x => !x.IsReadAll, () =>
        {
            RuleFor(x => x.NotificationIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("NotificationIds is required");
            
            RuleForEach(x => x.NotificationIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("NotificationId cannot be null or empty")
                .MaximumLength(50)
                .WithMessage("NotificationId is invalid");
        });
    }
}