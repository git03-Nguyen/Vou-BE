using FluentValidation;

namespace EventService.Features.Commands.CounterPartCommands.RedeemVoucher;

public class RedeemVoucherValidator : AbstractValidator<RedeemVoucherCommand>
{
    public RedeemVoucherValidator()
    {
        RuleFor(x => x.VoucherToPlayerId)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("VoucherToPlayerId is required");
    }
}
