using FluentValidation;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.Products;

public class ProductSaveRequestValidator : AbstractValidator<ProductSaveRequest>
{
    public ProductSaveRequestValidator()
    {
        RuleFor(p => p.Id)
            .Must(p => !p.HasValue || p <= 0)
            .WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductSaveRequest.Id),
                "1"));

        RuleFor(p => p.Name)
            .MaximumLength(100).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductSaveRequest.Name),
                "100"));

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductSaveRequest.Description),
                "1000"));

        RuleFor(p => p.CategoryId)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductSaveRequest.CategoryId),
                "1"));

        RuleFor(p => p.CategoryName)
            .MaximumLength(100).WithMessage(string.Format(
                ErrorMessages.GreaterThanError,
                nameof(ProductSaveRequest.Name),
                "100"));

        RuleFor(p => p.Amount)
            .GreaterThanOrEqualTo(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductSaveRequest.CategoryId),
                "0"));

        RuleFor(p => p.DateCreated)
            .Must(p => p >= new DateTime(2020, 6, 1))
            .WithMessage(string.Format(ErrorMessages.PastError, nameof(ProductSaveRequest.DateCreated), new DateTime(2020, 6, 1).ToString("d")));

        RuleForEach(p => p.CategoryParametersValuesIds)
            .GreaterThan(0).WithMessage(string.Format(
                ErrorMessages.LessThanError,
                nameof(ProductSaveRequest.Id),
                "1"));
    }
}