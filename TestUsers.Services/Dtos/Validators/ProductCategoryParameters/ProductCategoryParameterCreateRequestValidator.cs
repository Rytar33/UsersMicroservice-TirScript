using FluentValidation;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.ProductCategoryParameters;

public class ProductCategoryParameterCreateRequestValidator : AbstractValidator<ProductCategoryParameterCreateRequest>
{
    public ProductCategoryParameterCreateRequestValidator()
    {
        RuleFor(p => p.ProductCategoryId)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParameterCreateRequest.ProductCategoryId),
                "1"));

        RuleFor(p => p.Name)
            .MaximumLength(100).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductCategoryParameterCreateRequest.Name),
                "100"));

        RuleForEach(p => p.Values)
            .MaximumLength(250).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductCategoryParameterCreateRequest.Values),
                "250"));
    }
}
