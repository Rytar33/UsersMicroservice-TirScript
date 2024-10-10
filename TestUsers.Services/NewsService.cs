using TestUsers.Data.Models;
using TestUsers.Services.Extensions;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.News;
using TestUsers.Services.Dtos.Pages;
using TestUsers.Services.Dtos.Validators.News;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Exceptions;
using TestUsers.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TestUsers.Services;

public class NewsService(DataContext db) : INewsService
{
    public async Task<NewsListResponse> GetList(NewsListRequest request, CancellationToken cancellationToken = default)
    {
        await new NewsListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var newsForConditions = db.News.AsQueryable();

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
            newsForConditions = newsForConditions.GetPage(request.Page);

        var newsItems = newsForConditions.Select(n => 
            new NewsListItem(
                n.Id,
                n.Title,
                n.DateCreated,
                n.AuthorId)).ToList();

        return new NewsListResponse(newsItems, new PageResponse(countNews, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<NewsDetailResponse> GetDetail(int newsId, CancellationToken cancellationToken = default)
    {
        var news = await db.News.FindAsync([newsId], cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));

        var dbNewsTags = await db.NewsTagRelation.Where(x => x.NewsId == newsId)
            .Select(x => new NewsTagResponse(x.NewsTagId, x.NewsTag.Name)).ToListAsync(cancellationToken);

        return new NewsDetailResponse(news.Id, news.Title, news.Description, news.DateCreated, dbNewsTags);
    }

    public async Task<BaseResponse> Create(NewsCreateRequest request, Guid? sessionId = null,  CancellationToken cancellationToken = default)
    {
        await new NewsCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        if (await db.News.AnyAsync(n => n.Title == request.Title, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));

        var news = new News(request.Title, request.Description, DateTime.UtcNow, request.AuthorId);

        if (sessionId.HasValue)
        {
            var user = await db.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            news.AuthorId = user.UserId;
        }

        await db.News.AddAsync(news, cancellationToken);

        var newsTagsName = request.Tags.Split(", ").ToList();

        // Получаем существующие теги
        var existingTags = await db.NewsTag
            .Where(t => newsTagsName.Contains(t.Name))
            .ToListAsync(cancellationToken);

        // Добавляем новые теги
        var tagsToAdd = newsTagsName
            .Where(nt => !existingTags.Any(et => et.Name.Equals(nt, StringComparison.OrdinalIgnoreCase)))
            .Select(nt => new NewsTag(nt))
            .ToList();

        if (tagsToAdd.Count > 0)
            await db.NewsTag.AddRangeAsync(tagsToAdd, cancellationToken);

        // Создание новых связей для новости
        var allTags = existingTags.Concat(tagsToAdd).ToList();
        var newTagRelations = allTags
            .Select(et => new NewsTagRelation(news.Id, et.Id))
            .ToList();

        if (newTagRelations.Count > 0)
            await db.NewsTagRelation.AddRangeAsync(newTagRelations, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();

    }

    public async Task<BaseResponse> Edit(NewsEditRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        // Валидация запроса
        await new NewsEditRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        // Получаем новость по идентификатору
        var news = await db.News.FindAsync([ request.Id ], cancellationToken)
             ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));

        // Проверяем на совпадение заголовков
        if (await db.News.AnyAsync(n => n.Title == request.Title && n.Id != request.Id, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));

        if (sessionId.HasValue)
        {
            var user = await db.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (user.Id != news.AuthorId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }

        // Обновляем данные новости
        if (sessionId == null)
            news.AuthorId = request.AuthorId;
        news.Title = request.Title;
        news.Description = request.Description;

        // Получаем текущие теги
        var dbNewsTags = await db.NewsTagRelation
            .Where(x => x.NewsId == request.Id)
            .Select(x => new { x.NewsTagId, x.NewsTag.Name, x.Id })
            .ToListAsync(cancellationToken);

        // Получаем список тегов из запроса
        var newsTagsName = request.Tags.Split(", ").ToList();

        // Удаляем теги, которые отсутствуют в новом запросе
        var tagsToRemoveIds = dbNewsTags
            .Where(tr => !newsTagsName.Contains(tr.Name))
            .Select(tr => tr.NewsTagId)
            .ToList();

        var relationNewsTagsRemovedIds = dbNewsTags
            .Where(tr => !newsTagsName.Contains(tr.Name))
            .Select(tr => tr.Id)
            .ToList();

        if (tagsToRemoveIds.Count > 0)
        {
            // Удаление связей для тегов, которые больше не нужны
            db.NewsTagRelation.RemoveRange(
                db.NewsTagRelation.Where(ntr => tagsToRemoveIds.Contains(ntr.NewsTagId) && news.Id == ntr.NewsId));

            // Удаляем теги, которые больше не используются ни в одной другой новости
            db.NewsTag.RemoveRange(
                db.NewsTag.Where(t => tagsToRemoveIds.Contains(t.Id) &&
                                      db.NewsTagRelation.Any(ntr => relationNewsTagsRemovedIds.Contains(ntr.Id))));
        }

        // Проверяем существующие теги
        var existingTags = await db.NewsTag
            .Where(t => newsTagsName.Contains(t.Name))
            .ToListAsync(cancellationToken);

        // Добавляем новые теги
        var tagsToAdd = newsTagsName
            .Where(nt => !existingTags.Any(et => et.Name.Equals(nt, StringComparison.OrdinalIgnoreCase)))
            .Select(nt => new NewsTag(nt))
            .ToList();

        if (tagsToAdd.Count > 0)
            await db.NewsTag.AddRangeAsync(tagsToAdd, cancellationToken);

        // Создание новых связей для новости
        var allTags = existingTags.Concat(tagsToAdd).ToList();
        var newTagRelations = allTags
            .Where(et => !db.NewsTagRelation.Any(r => r.NewsId == request.Id && r.NewsTagId == et.Id))
            .Select(et => new NewsTagRelation(news.Id, et.Id))
            .ToList();

        if (newTagRelations.Count > 0)
            await db.NewsTagRelation.AddRangeAsync(newTagRelations, cancellationToken);

        // Сохраняем все изменения разом
        await db.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }

    public async Task Delete(int newsId, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var user = await db.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (!await db.News.AnyAsync(n => n.Id == newsId && user.UserId == n.AuthorId, cancellationToken))
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        if (!db.Database.IsInMemory())
        {
            var rowsRemoved = await db.News.Where(n => n.Id == newsId).ExecuteDeleteAsync(cancellationToken);
            if (rowsRemoved == 0)
                throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
        }
        else
        {
            var news = await db.News.FindAsync([newsId], cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
            db.News.Remove(news);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}