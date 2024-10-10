using TestUsers.Services.Dtos.News;
using TestUsers.Services.Dtos;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class NewsWsController(INewsService newsService) : BaseWsController
{
    public async Task<NewsListResponse> GetList(NewsListRequest request)
    {
        return await newsService.GetList(request);
    }

    public async Task<NewsDetailResponse> GetDetail(NewsDetailRequest request)
    {
        return await newsService.GetDetail(request.IdNews);
    }

    public async Task<BaseResponse> Create(NewsCreateRequest request)
    {
        return await newsService.Create(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Edit(NewsEditRequest request)
    {
        return await newsService.Edit(request, Socket.SessionId);
    }

    public async Task<bool> Delete(NewsDeleteRequest request)
    {
        await newsService.Delete(request.IdNews, Socket.SessionId);
        return true;
    }
}