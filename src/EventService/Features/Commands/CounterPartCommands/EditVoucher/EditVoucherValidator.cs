using FluentValidation;
using Shared.Common;

namespace EventService.Features.Commands.CounterPartCommands.EditVoucher;

public class EditVoucherValidator : AbstractValidator<EditVoucherCommand>
{
    public EditVoucherValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Id is required");

        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500)
            .Matches(Regexes.VALID_URL)
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("ImageUrl must be a valid URL and cannot exceed 500 characters");

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .When(x => x.Title is not null)
            .WithMessage("Title cannot be empty")
            .MaximumLength(Constants.VoucherTitleMaxLength)
            .When(x => x.Title is not null)
            .WithMessage($"Title cannot exceed {Constants.VoucherTitleMaxLength} characters");

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .When(x => x.Value.HasValue)
            .WithMessage("Value must be between 0 and 100");

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
            .When(x => x.RedemptionInstructions is not null)
            .WithMessage("RedemptionInstructions cannot exceed 500 characters");
    }
}
