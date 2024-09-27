using AutoMapper;
using Models;
using Models.Exceptions;
using Models.Extensions;
using Models.Validations;
using Services.Dtos;
using Services.Dtos.News;
using Services.Dtos.Pages;
using Services.Dtos.Validators.News;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class NewsService(
    IMapper mapper,
    INewsRepository newsRepository,
    INewsTagRepository newsTagRepository,
    INewsTagRelationRepository newsTagRelationRepository) : INewsService
{
    public async Task<NewsListResponse> GetList(NewsListRequest request, CancellationToken cancellationToken = default)
    {
        _ = new NewsListRequestValidator().ValidateWithErrors(request);
        var news = await newsRepository.GetListByExpression(null, cancellationToken, n => n.Tags);
        var newsForConditions = news.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            newsForConditions = newsForConditions
                .Where(n => 
                    n.Tags.Select(t => t.NewsTag.Name).Contains(request.Search)
                    || n.Title.Contains(request.Search)
                    || n.Description.Contains(request.Search));

        if (request.TagId.HasValue)
            newsForConditions = newsForConditions.Where(x => x.Tags.Select(t => t.NewsTagId).Contains(request.TagId.Value));

        var countNews = newsForConditions.Count();

        if (request.Page != null)
            newsForConditions = newsForConditions
                .Skip((request.Page.Page - 1) * request.Page.PageSize)
                .Take(request.Page.PageSize);

        var newsItems = newsForConditions.Select(u => mapper.Map<News, NewsListItem>(u)).ToList();

        return new NewsListResponse(newsItems, new PageResponse(countNews, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<NewsDetailResponse> GetDetail(int newsId, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByExpression(n => n.Id == newsId, cancellationToken);
        if (news == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
        return mapper.Map<News, NewsDetailResponse>(news);
    }

    public async Task<BaseResponse> Create(NewsCreateRequest request, CancellationToken cancellationToken = default)
    {
        _ = new NewsCreateRequestValidator().ValidateWithErrors(request);
        var tags = mapper.Map<string, List<NewsTag>>(request.Tags);
        var news = mapper.Map<NewsCreateRequest, News>(request);
        try
        {
            await newsTagRelationRepository.StartTransaction(cancellationToken);
            foreach (var tag in tags)
            {
                var gotTag = await newsTagRepository.GetByExpression(t => t.Name == tag.Name, cancellationToken);
                if (gotTag == null)
                    await newsTagRepository.CreateAsync(tag, cancellationToken);
            }

            await newsTagRepository.SaveChangesAsync(cancellationToken);

            await newsRepository.CreateAsync(news, cancellationToken);

            await newsRepository.SaveChangesAsync(cancellationToken);

            foreach (var tag in tags)
            {
                var gotTag = await newsTagRepository.GetByExpression(t => t.Name == tag.Name, cancellationToken)!;
                tag.Id = gotTag!.Id;
                await newsTagRelationRepository.CreateAsync(new NewsTagRelation(news.Id, tag.Id), cancellationToken);
            }

            await newsTagRelationRepository.SaveChangesAsync(cancellationToken);
            await newsTagRelationRepository.CommitTransaction(cancellationToken);
            return new BaseResponse();
        }
        catch (Exception)
        {
            await newsTagRelationRepository.RollBackTransaction(cancellationToken);
            throw;
        }
    }

    public async Task<BaseResponse> Edit(NewsEditRequest request, CancellationToken cancellationToken = default)
    {
        _ = new NewsEditRequestValidator().ValidateWithErrors(request);
        var tags = mapper.Map<string, List<NewsTag>>(request.Tags);
        var news = mapper.Map<NewsEditRequest, News>(request);
        try
        {
            await newsTagRelationRepository.StartTransaction(cancellationToken);

            foreach (var tag in tags)
            {
                var gotTag = await newsTagRepository.GetByExpression(t => t.Name == tag.Name, cancellationToken);
                if (gotTag != null) continue;
                await newsTagRepository.CreateAsync(tag, cancellationToken);
                await newsTagRepository.SaveChangesAsync(cancellationToken); // Сохраняем изменения для получения Id
            }

            foreach (var tag in tags)
            {
                var gotTag = await newsTagRepository.GetByExpression(t => t.Name == tag.Name, cancellationToken);
                if (gotTag == null)
                {
                    throw new InvalidOperationException($"Tag with name {tag.Name} should have been created but was not found.");
                }
                tag.Id = gotTag.Id;

                await newsTagRelationRepository.CreateAsync(new NewsTagRelation(news.Id, tag.Id), cancellationToken);
            }

            await newsTagRelationRepository.SaveChangesAsync(cancellationToken);

            foreach (var newsTag in await newsTagRepository.GetListByExpression(nt => !tags.Contains(nt) && nt.News.Any(ntr => ntr.NewsId == news.Id && nt.Id == ntr.NewsTagId), cancellationToken))
            {
                var newsTagRelations = await newsTagRelationRepository.GetListByExpression(ntr => ntr.NewsTagId == newsTag.Id, cancellationToken);
                if (newsTagRelations.Count == 0)
                    newsTagRepository.Delete(newsTag);
                foreach (var newsTagRelation in newsTagRelations)
                {
                    newsTagRelationRepository.Delete(newsTagRelation);
                }
            }

            await newsTagRelationRepository.SaveChangesAsync(cancellationToken);

            await newsRepository.UpdateAsync(news, cancellationToken);

            await newsRepository.SaveChangesAsync(cancellationToken);

            await newsTagRelationRepository.CommitTransaction(cancellationToken);

            return new BaseResponse();
        }
        catch (Exception)
        {
            await newsTagRelationRepository.RollBackTransaction(cancellationToken);
            throw;
        }
    }

    public async Task Delete(int newsId, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByExpression(n => n.Id == newsId, cancellationToken);
        if (news == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
        newsRepository.Delete(news);
        await newsRepository.SaveChangesAsync(cancellationToken);
    }
}