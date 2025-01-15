using System.Data;
using FluentValidation;
using Shared.Common;

namespace EventService.Features.Commands.PlayerCommands.BuyVoucher;

public class BuyVoucherValidator : AbstractValidator<BuyVoucherCommand>
{
    public BuyVoucherValidator()
    {
        RuleFor(x => x.EventId)
            .NotNull()
            .NotEmpty()
            .WithMessage("EventId is required");

    }
}