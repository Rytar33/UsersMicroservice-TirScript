using FluentValidation;
using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.UserLanguages;

public class AddLanguageToUserValidator : AbstractValidator<AddLanguageToUser>
{
    public AddLanguageToUserValidator()
    {
        RuleFor(p => p.UserId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(AddLanguageToUser.UserId), 0));

        RuleFor(p => p.LanguageId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(AddLanguageToUser.LanguageId), 0));

        RuleFor(p => p.DateLearn)
            .LessThanOrEqualTo(DateTime.Now).WithMessage(string.Format(ErrorMessages.FutureError, nameof(AddLanguageToUser.DateLearn)));
    }
}