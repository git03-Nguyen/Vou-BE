using FluentValidation;
using Shared.Common;

namespace EventService.Features.Commands.CounterPartCommands.CreateQuizSet;

public class CreateQuizSetValidator : AbstractValidator<CreateQuizSetCommand>
{
    public CreateQuizSetValidator()
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
        
        RuleFor(x => x.Quizes)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Quizes is required");
        
        RuleForEach(x => x.Quizes)
            .Must(x => x.Text.Length is > 0 and <= 100)
            .WithMessage("Text is required and must not exceed 100 characters")
            .Must(x => 
                x.AnswerA.Length is > 0 and <= 50 &&
                x.AnswerB.Length is > 0 and <= 50 &&
                x.AnswerC.Length is > 0 and <= 50 &&
                x.AnswerD.Length is > 0 and <= 50)
            .WithMessage("Answers are required and must not exceed 50 characters")
            .Must(x => x.Answer is > 0 and <= 3)
            .WithMessage("Answer is required and must be between 0 and 3");
    }
}