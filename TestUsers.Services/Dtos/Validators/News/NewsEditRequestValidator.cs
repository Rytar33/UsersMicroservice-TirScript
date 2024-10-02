using FluentValidation;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.News;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.News;

public class NewsEditRequestValidator : AbstractValidator<NewsEditRequest>
{
    public NewsEditRequestValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsEditRequest.Id), "0"));

        RuleFor(p => p.AuthorId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsEditRequest.AuthorId), "0"));

        RuleFor(p => p.Title)
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.GreaterThanError, nameof(NewsEditRequest.Title), "50"));

        RuleFor(p => p.Description)
            .MinimumLength(50).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsEditRequest.Description), "50"))
            .MaximumLength(1000).WithMessage(string.Format(ErrorMessages.GreaterThanError, nameof(NewsEditRequest.Description), "1000"));

        RuleForEach(p => SplitStringToArray(p.Tags))
            .Must(p => p.Length <= 50).WithMessage(string.Format(ErrorMessages.GreaterThanError, nameof(NewsTag.Name), "50"))
            .OverridePropertyName(nameof(NewsEditRequest.Tags));
    }

    private static string[] SplitStringToArray(string value)
        => value.Split(", ");
}