using FluentValidation;
using Models.Validations;
using Services.Dtos.UserContacts;

namespace Services.Dtos.Validators.UserContacts;

public class UserContactsSaveRequestValidator : AbstractValidator<UserContactsSaveRequest>
{
    public UserContactsSaveRequestValidator()
    {
        RuleFor(p => p.UserId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(UserContactsSaveRequest.UserId), "0"));

        RuleForEach(p => p.Contacts)
            .Where(p => p.Name.Length < 50 && p.Value.Length < 250);
    }
}