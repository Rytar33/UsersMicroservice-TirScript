using FluentValidation;
using Models.Validations;
using Services.Dtos.Users;

namespace Services.Dtos.Validators;

public class UsersListRequestValidator : AbstractValidator<UsersListRequest>
{
    public UsersListRequestValidator()
    {
        RuleFor(p => p.Page.Page)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(UsersListRequest.Page.Page), "0"));

        RuleFor(p => p.Page.PageSize)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(UsersListRequest.Page.PageSize), "0"));
    }
}