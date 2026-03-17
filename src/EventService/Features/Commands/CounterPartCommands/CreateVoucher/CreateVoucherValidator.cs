using FluentValidation;
using Shared.Common;

namespace EventService.Features.Commands.CounterPartCommands.CreateVoucher;

public class CreateVoucherValidator : AbstractValidator<CreateVoucherCommand>
{
    public CreateVoucherValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(Constants.VoucherTitleMaxLength)
            .WithMessage($"Title is required and must not exceed {Constants.VoucherTitleMaxLength} characters");
        
        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(Regexes.VALID_URL)
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("ImageUrl must be a valid URL");
        
        RuleFor(x => x.Value) 
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Discount value must be between 0 and 100");

        RuleFor(x => x.TotalQuantity)
            .GreaterThan(0)
            .When(x => x.TotalQuantity.HasValue)
            .WithMessage("TotalQuantity must be greater than 0");

        RuleFor(x => x.ExpiredDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpiredDate.HasValue)
            .WithMessage("ExpiredDate must be in the future");

        RuleFor(x => x.RedemptionInstructions)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.RedemptionInstructions))
            .WithMessage("RedemptionInstructions must not exceed 500 characters");
    }
}
