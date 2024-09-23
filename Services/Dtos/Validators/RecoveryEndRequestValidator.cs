using FluentValidation;
using Models.Validations;
using Services.Dtos.Users.Recoveries;

namespace Services.Dtos.Validators;

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