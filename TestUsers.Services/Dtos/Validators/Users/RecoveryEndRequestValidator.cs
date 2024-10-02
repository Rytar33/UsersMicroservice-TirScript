using FluentValidation;
using TestUsers.Services.Dtos.Users.Recoveries;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Users;

public class RecoveryEndRequestValidator : AbstractValidator<RecoveryEndRequest>
{
    public RecoveryEndRequestValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.EmptyError, nameof(RecoveryEndRequest.Email)))
            .Matches(RegexPatterns.EmailPattern).WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p.RecoveryTokenRequest)
            .Length(6).WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p)
            .Must(p => p.NewPassword == p.RepeatNewPassword)
            .WithMessage(string.Format(ErrorMessages.NotCoincideError, nameof(RecoveryEndRequest.RepeatNewPassword)));
    }
}