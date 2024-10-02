using FluentValidation;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Products;

public class ProductListRequestValidator : AbstractValidator<ProductListRequest>
{
    public ProductListRequestValidator()
    {
        RuleFor(p => p.CategoryId)
            .Must(p => !p.HasValue || p <= 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.CategoryId),
                "1"));

        RuleForEach(p => p.CategoryParametersValuesIds)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.CategoryParametersValuesIds),
                "1"));

        RuleFor(p => p.FromAmount)
            .Must(p => !p.HasValue || p.Value < 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.FromAmount),
                "0"));

        RuleFor(p => p.ToAmount)
            .Must(p => !p.HasValue || p.Value < 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.FromAmount),
                "0"));

        RuleFor(p => p)
            .Must(p => (!p.FromAmount.HasValue || !p.ToAmount.HasValue)
                       || p.FromAmount.Value <= p.ToAmount.Value)
            .WithMessage(ErrorMessages.RegexError);

        RuleFor(p => p.Page)
            .Must(p => p == null || p.Page <= 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.Page.Page),
                "1"));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.PageSize <= 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductListRequest.Page.PageSize),
                "1"));
    }
}