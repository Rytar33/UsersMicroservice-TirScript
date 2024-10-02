using FluentValidation;
using TestUsers.Services.Dtos.ProductCategory;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.ProductCategories;

public class ProductCategoryUpdateRequestValidator : AbstractValidator<ProductCategoryUpdateRequest>
{
    public ProductCategoryUpdateRequestValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryUpdateRequest.Id),
                "1"));

        RuleFor(p => p.Name)
            .MaximumLength(100).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductCategoryUpdateRequest.Name),
                "100"));

        RuleFor(p => p.ParentCategoryId)
            .Must(p => !p.HasValue || p <= 0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryUpdateRequest.ParentCategoryId),
                "1"));
    }
}