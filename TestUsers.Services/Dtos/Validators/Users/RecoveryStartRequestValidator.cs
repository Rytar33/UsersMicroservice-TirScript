using FluentValidation;
using TestUsers.Services.Dtos.Users.Recoveries;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Users;

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