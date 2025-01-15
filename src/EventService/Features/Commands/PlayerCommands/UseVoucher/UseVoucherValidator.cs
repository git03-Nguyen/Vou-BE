using FluentValidation;

namespace EventService.Features.Commands.PlayerCommands.UseVoucher;

public class UseVoucherValidator : AbstractValidator<UseVoucherCommand>
{
    public UseVoucherValidator()
    {
        RuleFor(x => x.VoucherToPlayerId)
            .NotNull()
            .NotEmpty()
            .WithMessage("VoucherToPlayerId is required")
            .MaximumLength(100)
            .WithMessage("VoucherToPlayerId must not exceed 100 characters");
    }
}