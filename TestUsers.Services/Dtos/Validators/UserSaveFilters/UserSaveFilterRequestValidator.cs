using FluentValidation;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.UserSaveFilters;

public class UserSaveFilterRequestValidator : AbstractValidator<UserSaveFilterRequest>
{
    public UserSaveFilterRequestValidator()
    {
        RuleFor(p => p.UserId)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.UserId),
                "1"));

        RuleFor(p => p.SaveFilterName)
            .MaximumLength(100)
            .WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(UserSaveFilterRequest.SaveFilterName),
                "100"));

        RuleFor(p => p.CategoryParametersValuesIds)
            .Must(ps => ps == null || ps.All(p => p > 0))
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.UserId),
                "1"));

        RuleFor(p => p.CategoryParametersValuesIds)
            .Must(p => p == null || !p.GroupBy(v => v).Any(v => v.Count() > 1))
            .WithMessage(string.Format(
                ErrorMessages.CoincideError,
                nameof(UserSaveFilterRequest.CategoryParametersValuesIds),
                nameof(ProductCategoryParameterValue.Id)));

        RuleFor(p => p.Search)
            .Must(p => string.IsNullOrWhiteSpace(p) || p.Length <= 200)
            .WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(UserSaveFilterRequest.Search),
                "200"));

        RuleFor(p => p.CategoryId)
            .Must(p => !p.HasValue || p > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.CategoryId),
                "1"));

        RuleForEach(p => p.CategoryParametersValuesIds)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.CategoryParametersValuesIds),
                "1"));

        RuleFor(p => p.FromAmount)
            .Must(p => !p.HasValue || p.Value > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.FromAmount),
                "0"));

        RuleFor(p => p.ToAmount)
            .Must(p => !p.HasValue || p.Value > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.FromAmount),
                "0"));

        RuleFor(p => p)
            .Must(p => (!p.FromAmount.HasValue || !p.ToAmount.HasValue)
                       || p.FromAmount.Value <= p.ToAmount.Value)
            .WithMessage(ErrorMessages.RegexError);
    }
}