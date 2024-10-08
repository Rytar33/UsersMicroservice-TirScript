using FluentValidation;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.ProductCategoryParameters;

public class ProductCategoryParameterUpdateRequestValidator : AbstractValidator<ProductCategoryParameterUpdateRequest>
{
    public ProductCategoryParameterUpdateRequestValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParameterUpdateRequest.Id),
                1));

        RuleFor(p => p.ProductCategoryId)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductCategoryParameterUpdateRequest.ProductCategoryId),
                1));

        RuleFor(p => p.Name)
            .MaximumLength(100).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductCategoryParameterUpdateRequest.Name),
                100));

        RuleForEach(p => p.Values)
            .MaximumLength(250).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductCategoryParameterUpdateRequest.Values),
                250));
    }
}