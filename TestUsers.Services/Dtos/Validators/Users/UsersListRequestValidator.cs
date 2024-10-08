using FluentValidation;
using TestUsers.Services.Dtos.Users;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Users;

public class UsersListRequestValidator : AbstractValidator<UsersListRequest>
{
    public UsersListRequestValidator()
    {
        RuleFor(p => p.Page)
            .Must(p => p == null || p.Page > 0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(UsersListRequest.Page.Page), 0));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.PageSize > 0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(UsersListRequest.Page.PageSize), 0));
    }
}