using FluentValidation;

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
            .NotEmpty()
            .WithMessage("ImageUrl cannot be empty")
            .MaximumLength(500)
            .WithMessage("ImageUrl cannot exceed 500 characters");

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Title cannot be empty")
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Value)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Value must be between 0 and 100");
    }
}