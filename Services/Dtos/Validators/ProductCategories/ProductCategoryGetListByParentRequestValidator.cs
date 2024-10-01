using FluentValidation;
using Models.Validations;
using Services.Dtos.ProductCategory;

namespace Services.Dtos.Validators.ProductCategories;

public class ProductCategoryGetListByParentRequestValidator : AbstractValidator<ProductCategoryGetListByParentRequest>
{
    public ProductCategoryGetListByParentRequestValidator()
    {
        RuleFor(p => p.ParentCategoryId)
            .Must(p => !p.HasValue || p <= 0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryGetListByParentRequest.ParentCategoryId),
                "1"));
    }
}