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

public class NewsService(DataContext _db) : INewsService
{
    public async Task<NewsListResponse> GetList(NewsListRequest request, CancellationToken cancellationToken = default)
    {
        await new NewsListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var newsForConditions = _db.News.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            newsForConditions = newsForConditions
                .Where(n =>
                    n.Title.Contains(request.Search)
                    || n.Description.Contains(request.Search)
                    || n.Tags.Select(t => t.NewsTag.Name).Contains(request.Search));

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
        var news = await _db.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == newsId, cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));

        var dbNewsTags = await _db.NewsTagRelation.Where(x => x.NewsId == newsId)
            .Select(x => new NewsTagResponse(x.NewsTagId, x.NewsTag.Name)).ToListAsync(cancellationToken);

        return new NewsDetailResponse(news.Id, news.Title, news.Description, news.DateCreated, dbNewsTags);
    }

    public async Task<BaseResponse> Create(NewsCreateRequest request, Guid? sessionId = null,  CancellationToken cancellationToken = default)
    {
        await new NewsCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        if (await _db.News.AnyAsync(n => n.Title == request.Title, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));

        var news = new News(request.Title, request.Description, DateTime.UtcNow, request.AuthorId);

        if (sessionId.HasValue)
        {
            var user = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            news.AuthorId = user.UserId;
        }

        await _db.News.AddAsync(news, cancellationToken);

        var newsTagsName = request.Tags.Split(", ").ToList();

        // Получаем существующие теги
        var existingTags = await _db.NewsTag
            .Where(t => newsTagsName.Contains(t.Name))
            .ToListAsync(cancellationToken);

        // Добавляем новые теги
        var tagsToAdd = newsTagsName
            .Where(nt => !existingTags.Any(et => et.Name.Equals(nt, StringComparison.OrdinalIgnoreCase)))
            .Select(nt => new NewsTag(nt))
            .ToList();

        if (tagsToAdd.Count > 0)
            await _db.NewsTag.AddRangeAsync(tagsToAdd, cancellationToken);

        // Создание новых связей для новости
        var allTags = existingTags.Concat(tagsToAdd).ToList();
        var newTagRelations = allTags
            .Select(et => new NewsTagRelation(news.Id, et.Id))
            .ToList();

        if (newTagRelations.Count > 0)
            await _db.NewsTagRelation.AddRangeAsync(newTagRelations, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();

    }

    public async Task<BaseResponse> Edit(NewsEditRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        // Валидация запроса
        await new NewsEditRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        // Получаем новость по идентификатору
        var news = await _db.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken)
             ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));

        // Проверяем на совпадение заголовков
        if (await _db.News.AnyAsync(n => n.Title == request.Title && n.Id != request.Id, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));

        if (sessionId.HasValue)
        {
            var user = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken)
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
        var dbNewsTags = await _db.NewsTagRelation
            .Where(x => x.NewsId == request.Id)
            .Select(x => new { x.NewsTagId, x.NewsTag.Name, x.Id, x.NewsId })
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
            _db.NewsTagRelation.RemoveRange(
                _db.NewsTagRelation.Where(ntr => tagsToRemoveIds.Contains(ntr.NewsTagId) && news.Id == ntr.NewsId));

            // Получаем все теги, которые нужно удалить
            var tagsToRemove = await _db.NewsTag
                .Where(t => tagsToRemoveIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            // Удаляем теги, которые больше не используются ни в одной другой новости
            var tagsToRemoveWithRelations = tagsToRemove
                .Where(t => dbNewsTags.Any(ntr => relationNewsTagsRemovedIds.Contains(ntr.Id)))
                .ToList();

            _db.NewsTag.RemoveRange(tagsToRemoveWithRelations);

        }

        // Проверяем существующие теги
        var existingTags = await _db.NewsTag
            .Where(t => newsTagsName.Contains(t.Name))
            .ToListAsync(cancellationToken);

        // Добавляем новые теги
        var tagsToAdd = newsTagsName
            .Where(nt => !existingTags.Any(et => et.Name.Equals(nt, StringComparison.OrdinalIgnoreCase)))
            .Select(nt => new NewsTag(nt))
            .ToList();

        if (tagsToAdd.Count > 0)
            await _db.NewsTag.AddRangeAsync(tagsToAdd, cancellationToken);

        // Создание новых связей для новости
        var allTags = existingTags.Concat(tagsToAdd).ToList();
        var newTagRelations = allTags
            .Where(et => !dbNewsTags.Any(r => r.NewsId == request.Id && r.NewsTagId == et.Id))
            .Select(et => new NewsTagRelation(news.Id, et.Id))
            .ToList();

        if (newTagRelations.Count > 0)
            await _db.NewsTagRelation.AddRangeAsync(newTagRelations, cancellationToken);

        // Сохраняем все изменения разом
        await _db.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }

    public async Task Delete(int newsId, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var user = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (!await _db.News.AnyAsync(n => n.Id == newsId && user.UserId == n.AuthorId, cancellationToken))
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        if (!_db.Database.IsInMemory())
        {
            var rowsRemoved = await _db.News.Where(n => n.Id == newsId).ExecuteDeleteAsync(cancellationToken);
            if (rowsRemoved == 0)
                throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
        }
        else
        {
            var news = await _db.News.FirstOrDefaultAsync(n => n.Id == newsId, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
            _db.News.Remove(news);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}