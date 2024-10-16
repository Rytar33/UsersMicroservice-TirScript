﻿using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.News;

namespace TestUsers.Services.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса новостей
/// </summary>
public interface INewsService
{
    /// <summary>
    /// Получить список новостей с постраничкой
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<NewsListResponse> GetList(NewsListRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить детальную страницу новости
    /// </summary>
    /// <param name="newsId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<NewsDetailResponse> GetDetail(int newsId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новость
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> Create(NewsCreateRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Изменить новость
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> Edit(NewsEditRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить новость
    /// </summary>
    /// <param name="newsId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Delete(int newsId, Guid? sessionId = null, CancellationToken cancellationToken = default);
}