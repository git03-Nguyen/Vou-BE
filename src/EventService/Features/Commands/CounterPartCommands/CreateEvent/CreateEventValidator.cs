using FluentValidation;
using Shared.Common;

namespace EventService.Features.Commands.CounterPartCommands.CreateEvent;

public class CreateEventValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Name is required and must not exceed 50 characters");
        
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Description is required and must not exceed 500 characters");
        
        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Matches(Regexes.VALID_URL)
            .WithMessage("ImageUrl is required and must be a valid URL");
        
        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .GreaterThan(DateTime.Now + TimeSpan.FromMinutes(1))
            .WithMessage("StartDate is required and must be greater than the current time");
        
        RuleFor(x => x.EndDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .GreaterThan(DateTime.Now + TimeSpan.FromMinutes(1))
            .WithMessage("EndDate is required and must be greater than the current time");
        
        When(x => x.ShakeSession != null, () =>
        {
            RuleFor(x => x.ShakeSession!.VoucherId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .WithMessage("VoucherId is required");
            
            RuleFor(x => x.ShakeSession!.Price)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Price is required and must be greater than 0");
            
            RuleFor(x => x.ShakeSession!.WinRate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .WithMessage("WinRate is required and must be between 0 and 100");
            
            RuleFor(x => x.ShakeSession!.AverageDiamond)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(9999)
                .WithMessage("AverageDiamonds is required and must be between 0 and 9999");
        });
        
        When(x => x.QuizSessions != null, () =>
        {
            RuleFor(x => x.QuizSessions) 
                .NotEmpty()
                .WithMessage("QuizSessions cannot be empty");
            
            RuleForEach(x => x.QuizSessions)
                .Cascade(CascadeMode.Stop)
                .Must(x => x.VoucherId.Length is > 0 and <= 50)
                .WithMessage("VoucherId is required and must not exceed 50 characters")
                .Must(x => x.QuizSetId.Length is > 0 and <= 50)
                .WithMessage("QuizSetId is required and must not exceed 50 characters")
                .Must(x => x.QuizSetId.Length is > 0 and <= 50)
                .WithMessage("QuizSetId is required and must not exceed 50 characters")
                .Must(x => x.TakeTop > 0)
                .WithMessage("TakeTop is required and must be greater than 0")
                .Must(x => x.StartTime > DateTime.Now + TimeSpan.FromMinutes(1))
                .WithMessage("StartTime is required and must be greater than the current time");
        });
    }
}