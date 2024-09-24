using FluentValidation;
using Models.Validations;
using Services.Dtos.Users.Recoveries;

namespace Services.Dtos.Validators;

public class RecoveryStartRequestValidator : AbstractValidator<RecoveryStartRequest>
{
    public RecoveryStartRequestValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.EmptyError, nameof(RecoveryStartRequest.Email)))
            .Matches(RegexPatterns.EmailPattern).WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p.RequestCode)
            .Must(code => code == null || code.Length == 6)
            .WithMessage(ErrorMessages.RegexError);
    }
}