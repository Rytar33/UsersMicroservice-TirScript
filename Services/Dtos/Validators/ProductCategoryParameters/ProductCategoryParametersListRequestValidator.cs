using FluentValidation;
using Models.Validations;
using Services.Dtos.ProductCategory;
using Services.Dtos.ProductCategoryParameters;

namespace Services.Dtos.Validators.ProductCategoryParameters;

public class ProductCategoryParametersListRequestValidator : AbstractValidator<ProductCategoryParametersListRequest>
{
    public ProductCategoryParametersListRequestValidator()
    {
        RuleFor(p => p.ProductCategoryId)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParametersListRequest.ProductCategoryId),
                "1"));
    }
}