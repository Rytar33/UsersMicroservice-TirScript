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
            .Select(x => new NewsTagResponse(x.NewsTagId, x.NewsTag.Name)).ToListAsync();

        return new NewsDetailResponse(news.Id, news.Title, news.Description, news.DateCreated, dbNewsTags);
    }

    public async Task<BaseResponse> Create(NewsCreateRequest request, CancellationToken cancellationToken = default)
    {
        await new NewsCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        if (await db.News.AnyAsync(n => n.Title == request.Title, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));

        var news = new News(request.Title, request.Description, DateTime.UtcNow, request.AuthorId);

        await db.News.AddAsync(news, cancellationToken);

        var newsTagsName = new List<string>(request.Tags.Split(", "));

        // Определяем существующие теги в базе, чтобы избежать их дублирования
        var existingTags = await db.NewsTag
            .Where(t => newsTagsName.Contains(t.Name))
            .ToListAsync();

        // Определяем новые теги для добавления
        var tagsToAdd = newsTagsName
            .Where(nt => !existingTags.Any(et => et.Name.Equals(nt, StringComparison.OrdinalIgnoreCase)))
            .Select(nt => new NewsTag(nt))
            .ToList();

        if (await db.NewsTag.AnyAsync(nt => tagsToAdd.Any(t => t.Name == nt.Name), cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(NewsTag.Name), nameof(NewsTag)));

        // Добавляем новые теги в базу данных
        if (tagsToAdd.Count != 0)
            await db.NewsTag.AddRangeAsync(tagsToAdd, cancellationToken);

        // Обновляем существующие теги и создаём новые связи для новости
        var allTags = existingTags.Concat(tagsToAdd).ToList();
        var newTagRelations = allTags
            .Select(et => new NewsTagRelation(news.Id, et.Id))
            .ToList();

        // Добавляем новые связи между новостью и тегами
        if (newTagRelations.Count != 0)
            await db.NewsTagRelation.AddRangeAsync(newTagRelations, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Edit(NewsEditRequest request, CancellationToken cancellationToken = default)
    {
        // Валидация запроса
        await new NewsEditRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        // Получаем новость по идентификатору
        var news = await db.News.FindAsync(new object[] { request.Id }, cancellationToken)
             ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));

        if (!await db.News.AnyAsync(n => n.Title == request.Title && n.Id != request.Id, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));

        // Обновляем данные новости
        news.AuthorId = request.AuthorId;
        news.Title = request.Title;
        news.Description = request.Description;

        // Получаем теги, связанные с новостью, с использованием одного запроса
        var dbNewsTags = await db.NewsTagRelation
            .Where(x => x.NewsId == request.Id)
            .Select(x => new { x.NewsTagId, x.NewsTag.Name })
            .ToListAsync();

        // Получаем список тегов из запроса
        var newsTagsName = new List<string>(request.Tags.Split(", "));

        // Определяем теги для удаления, которые не присутствуют в запросе
        var tagsToRemove = dbNewsTags
            .Where(tr => !newsTagsName.Contains(tr.Name))
            .Select(tr => new NewsTagRelation(request.Id, tr.NewsTagId))
            .ToList();

        // Удаляем теги, которые больше не нужны (удаление связей)
        if (tagsToRemove.Count != 0)
            db.NewsTagRelation.RemoveRange(tagsToRemove);

        // Удаляем теги, у которых больше нет связей
        var tagsToDelete = await db.NewsTag
            .Where(t => tagsToRemove.Any(tr => tr.NewsTagId == t.Id) &&
                        !db.NewsTagRelation.Any(r => r.NewsTagId == t.Id))
            .ToListAsync();

        if (tagsToDelete.Count != 0)
            db.NewsTag.RemoveRange(tagsToDelete);

        // Определяем существующие теги в базе, чтобы избежать их дублирования
        var existingTags = await db.NewsTag
            .Where(t => newsTagsName.Contains(t.Name))
            .ToListAsync();

        // Определяем новые теги для добавления
        var tagsToAdd = newsTagsName
            .Where(nt => !existingTags.Any(et => et.Name.Equals(nt, StringComparison.OrdinalIgnoreCase)))
            .Select(nt => new NewsTag(nt))
            .ToList();

        if (await db.NewsTag.AnyAsync(nt => tagsToAdd.Any(t => t.Name == nt.Name), cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(NewsTag.Name), nameof(NewsTag)));

        // Добавляем новые теги в базу данных
        if (tagsToAdd.Count != 0)
            await db.NewsTag.AddRangeAsync(tagsToAdd, cancellationToken);

        // Обновляем существующие теги и создаём новые связи для новости
        var allTags = existingTags.Concat(tagsToAdd).ToList();
        var newTagRelations = allTags
            .Select(et => new NewsTagRelation(news.Id, et.Id))
            .ToList();

        // Добавляем новые связи между новостью и тегами
        if (newTagRelations.Count != 0)
            await db.NewsTagRelation.AddRangeAsync(newTagRelations, cancellationToken);

        // Сохраняем изменения
        await db.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }

    public async Task Delete(int newsId, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await db.News.Where(n => n.Id == newsId).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
    }
}