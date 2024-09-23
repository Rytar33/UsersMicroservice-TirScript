using FluentValidation;
using Models.Validations;
using Services.Dtos.Users;

namespace Services.Dtos.Validators;

public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
{
    public UserCreateRequestValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.EmptyError, nameof(UserCreateRequest.Email)))
            .Matches(RegexPatterns.EmailPattern).WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p.FullName)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.EmptyError, nameof(UserCreateRequest.FullName)))
            .Matches(RegexPatterns.FullNamePattern).WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.EmptyError, nameof(UserCreateRequest.Password)))
            .MinimumLength(6)
            .MaximumLength(24).WithMessage(ErrorMessages.RegexError);
    }
}