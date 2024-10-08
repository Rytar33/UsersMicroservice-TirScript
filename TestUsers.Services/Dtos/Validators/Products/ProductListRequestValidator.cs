using FluentValidation;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Products;

public class ProductListRequestValidator : AbstractValidator<ProductListRequest>
{
    public ProductListRequestValidator()
    {
        RuleFor(p => p.UserId)
            .Must(p => !p.HasValue || p > 0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.UserId),
                1));

        RuleFor(p => p.FilterName)
            .MaximumLength(100)
            .WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductListRequest.FilterName),
                100));

        RuleFor(p => p)
            .Must(p => !p.SaveFilter || (!string.IsNullOrWhiteSpace(p.FilterName) && p.UserId.HasValue))
            .WithMessage(string.Format(ErrorMessages.SaveRequestError, nameof(ProductListRequest.SaveFilter)));

        RuleFor(p => p.CategoryParametersValuesIds)
            .Must(ps => ps == null || ps.All(p => p > 0))
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(UserSaveFilterRequest.UserId),
                1));

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
                nameof(ProductListRequest.Search),
                200));

        RuleFor(p => p.CategoryId)
            .Must(p => !p.HasValue || p > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.CategoryId),
                1));

        RuleForEach(p => p.CategoryParametersValuesIds)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.CategoryParametersValuesIds),
                1));

        RuleFor(p => p.FromAmount)
            .Must(p => !p.HasValue || p.Value > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.FromAmount),
                0));

        RuleFor(p => p.ToAmount)
            .Must(p => !p.HasValue || p.Value > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.FromAmount),
                0));

        RuleFor(p => p)
            .Must(p => (!p.FromAmount.HasValue || !p.ToAmount.HasValue)
                       || p.FromAmount.Value <= p.ToAmount.Value)
            .WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p.Page)
            .Must(p => p == null || p.Page > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.Page.Page),
                1));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.PageSize > 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.Page.PageSize),
                1));
    }
}