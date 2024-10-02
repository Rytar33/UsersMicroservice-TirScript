using FluentValidation;
using TestUsers.Services.Dtos.Users;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Users;

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