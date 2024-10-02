using FluentValidation;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.ProductCategoryParameters;

public class ProductCategoryParameterValuesListRequestValidator : AbstractValidator<ProductCategoryParameterValuesListRequest>
{
    public ProductCategoryParameterValuesListRequestValidator()
    {
        RuleFor(p => p.ProductCategoryParameterId)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParameterValuesListRequest.ProductCategoryParameterId),
                "1"));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.Page <= 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParameterValuesListRequest.Page.Page),
                "1"));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.PageSize <= 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParameterValuesListRequest.Page.PageSize),
                "1"));
    }
}