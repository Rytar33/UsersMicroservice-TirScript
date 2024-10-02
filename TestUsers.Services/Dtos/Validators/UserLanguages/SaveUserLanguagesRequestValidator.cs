using FluentValidation;
using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.UserLanguages;

public class SaveUserLanguagesRequestValidator : AbstractValidator<SaveUserLanguagesRequest>
{
    public SaveUserLanguagesRequestValidator()
    {
        RuleFor(p => p.UserId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(SaveUserLanguagesRequest.UserId), "0"));

        RuleForEach(p => p.Languages)
            .Must(p => p.LanguageId > 0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(SaveUserLanguageItem.LanguageId), "0"))
            .Must(p => p.DateLearn <= DateTime.Now).WithMessage(string.Format(ErrorMessages.FutureError, nameof(SaveUserLanguageItem.DateLearn)));
    }
}