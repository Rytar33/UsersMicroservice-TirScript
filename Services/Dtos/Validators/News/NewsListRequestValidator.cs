using FluentValidation;
using Services.Dtos.News;

namespace Services.Dtos.Validators.News;

public class NewsListRequestValidator : AbstractValidator<NewsListRequest>
{
    public NewsListRequestValidator()
    {

    }
}