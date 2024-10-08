using FluentValidation;
using TestUsers.Services.Dtos.News;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services.Dtos.Validators.News;

public class NewsListRequestValidator : AbstractValidator<NewsListRequest>
{
    public NewsListRequestValidator()
    {
        RuleFor(p => p.TagId)
            .Must(p => !p.HasValue || p > 0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsListRequest.TagId), 0));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.Page > 0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsListRequest.Page.Page), 0));

        RuleFor(p => p.Page)
            .Must(p => p == null || p.PageSize > 0).WithMessage(string.Format(ErrorMessages.LessThanError, nameof(NewsListRequest.Page.PageSize), 0));
    }
}