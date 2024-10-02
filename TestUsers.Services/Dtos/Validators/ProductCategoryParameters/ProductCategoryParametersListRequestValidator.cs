using FluentValidation;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.ProductCategoryParameters;

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