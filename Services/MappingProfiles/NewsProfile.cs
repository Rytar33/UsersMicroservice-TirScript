using AutoMapper;
using Models;
using Services.Dtos.News;

namespace Services.MappingProfiles;

public class NewsProfile : Profile
{
    public NewsProfile()
    {
        CreateMap<News, NewsListItem>();

        CreateMap<NewsTag, NewsTagResponse>();

        CreateMap<News, NewsDetailResponse>()
            .ForMember(p => p.Tags, f => f.MapFrom(p => p.Tags.Select(ntr => ntr.NewsTag)));

        CreateMap<NewsCreateRequest, News>();

        CreateMap<NewsEditRequest, News>();

        CreateMap<string, List<NewsTag>>()
            .ConvertUsing(src => StringToNewsTagList(src));
    }
    private static List<NewsTag> StringToNewsTagList(string source)
        => source.Split(", ")
            .Select(str => new NewsTag(str))
            .ToList();
}