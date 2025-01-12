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
            .MaximumLength(50)
            .WithMessage("Title is required and must not exceed 50 characters");
        
        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Matches(Regexes.VALID_URL)
            .WithMessage("ImageUrl is required and must be a valid URL");
        
        RuleFor(x => x.Value) 
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Discount value is required and must be between 0 and 100");
    }
}