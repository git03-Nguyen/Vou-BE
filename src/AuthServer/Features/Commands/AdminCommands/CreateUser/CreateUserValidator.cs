using AuthServer.Common;
using FluentValidation;
using Constants = Shared.Common.Constants;

namespace AuthServer.Features.Commands.AdminCommands.CreateUser;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("UserName is required")
            .Matches(Regexes.VALID_USERNAME)
            .WithMessage("UserName is invalid");
        
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Email is required")
            .Matches(Regexes.VALID_EMAIL)
            .WithMessage("Email is invalid");
        
        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("PhoneNumber is required")
            .Matches(Regexes.VALID_PHONE)
            .WithMessage("PhoneNumber is invalid");
        
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Password is required")
            .Matches(Regexes.VALID_PASSWORD)
            .WithMessage("Password is invalid");
        
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("FullName is required")
            .Matches(Regexes.VALID_FULLNAME)
            .WithMessage("FullName is invalid");
        
        RuleFor(x => x.Role)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(x => x == Constants.PLAYER || x == Constants.ADMIN || x == Constants.COUNTERPART)
            .WithMessage("Role is invalid");
        
        // When(x => x.AvatarImage is not null, () =>
        // {
        //     RuleFor(x => x.AvatarImage!.Length)
        //         .Cascade(CascadeMode.Stop)
        //         .LessThanOrEqualTo(1024 * 1024)
        //         .WithMessage("AvatarImage is too large");
        // });
        
        // For CounterPart
        When(x => x.Role == Constants.COUNTERPART, () =>
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name is required")
                .Matches(Regexes.VALID_COUNTERPART_NAME)
                .WithMessage("Name is invalid");
            
            RuleFor(x => x.Field)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .WithMessage("Field is required")
                .Matches(Regexes.VALID_COUNTERPART_FIELD)
                .WithMessage("Field is invalid");

            RuleFor(x => x.Addresses)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .WithMessage("Addresses is required");
            
            RuleFor(x => x.Addresses![0].AddressText)
                .NotEmpty()
                .Matches(Regexes.VALID_ADDRESS_TEXT)
                .WithMessage("AddressText is invalid");
        });
        
        // For Player
        When(x => x.Role == Constants.PLAYER, () =>
        {
            RuleFor(x => x.BirthDate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .WithMessage("BirthDate is required")
                .Must(x => x <= DateTime.Now)
                .WithMessage("BirthDate is invalid");

            RuleFor(x => x.Gender)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .IsInEnum()
                .WithMessage("BirthDate is required");
            
            When(x => x.FacebookUrl is not null, () =>
            {
                RuleFor(x => x.FacebookUrl)
                    .Cascade(CascadeMode.Stop)
                    .Matches(Regexes.VALID_FACEBOOK_URL)
                    .WithMessage("FacebookUrl is invalid");
            });
        });
    }
}