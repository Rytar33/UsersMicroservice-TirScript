using FluentValidation;
using Models;
using Models.Validations;
using Services.Dtos.News;

namespace Services.Dtos.Validators.News;

public class NewsCreateRequestValidator : AbstractValidator<NewsCreateRequest>
{
    public NewsCreateRequestValidator()
    {
        RuleFor(p => p.AuthorId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsCreateRequest.AuthorId), "0"));

        RuleFor(p => p.Title)
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.GreaterThanError, nameof(NewsCreateRequest.Title), "50"));

        RuleFor(p => p.Description)
            .MinimumLength(50).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsCreateRequest.Description), "50"))
            .MaximumLength(1000).WithMessage(string.Format(ErrorMessages.GreaterThanError, nameof(NewsCreateRequest.Description), "1000"));

        RuleForEach(p => SplitStringToArray(p.Tags))
            .Must(p => p.Length <= 50).WithMessage(string.Format(ErrorMessages.GreaterThanError, nameof(NewsTag.Name), "50"))
            .OverridePropertyName(nameof(NewsCreateRequest.Tags));
    }

    private static string[] SplitStringToArray(string value)
        => value.Split(", ");
}