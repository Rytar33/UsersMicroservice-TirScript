using FluentValidation;
using Models.Validations;
using Services.Dtos.Users;

namespace Services.Dtos.Validators;

public class UserEditRequestValidator : AbstractValidator<UserEditRequest>
{
    public UserEditRequestValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(UserEditRequest.Id), "0"));

        RuleFor(p => p.FullName)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.EmptyError, nameof(UserCreateRequest.FullName)))
            .Matches(RegexPatterns.FullNamePattern).WithMessage(ErrorMessages.RegexError);
    }
}