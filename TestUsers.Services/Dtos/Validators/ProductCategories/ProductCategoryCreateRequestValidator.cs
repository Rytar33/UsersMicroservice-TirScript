using FluentValidation;
using TestUsers.Services.Dtos.ProductCategory;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.ProductCategories;

public class ProductCategoryCreateRequestValidator : AbstractValidator<ProductCategoryCreateRequest>
{
    public ProductCategoryCreateRequestValidator()
    {
        RuleFor(p => p.ParentCategoryId)
            .Must(p => !p.HasValue || p <= 0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryCreateRequest.ParentCategoryId),
                "1"));

        RuleFor(p => p.Name)
            .MaximumLength(100).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductCategoryCreateRequest.Name),
                "100"));
    }
}