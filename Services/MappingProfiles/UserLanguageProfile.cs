using AutoMapper;
using Models;
using Services.Dtos.UserLanguages;

namespace Services.MappingProfiles;

public class UserLanguageProfile : Profile
{
    public UserLanguageProfile()
    {
        CreateMap<UserLanguage, UserLanguageItemResponse>()
            .ForMember(p => p.Name, ul => ul.MapFrom(p => p.Language.Name))
            .ForMember(p => p.Code, ul => ul.MapFrom(p => p.Language.Code));

        CreateMap<SaveUserLanguagesRequest, List<UserLanguage>>()
            .ForAllMembers(p => p.MapFrom(ul => ul.Languages));

        CreateMap<AddLanguageToUser, UserLanguage>();
    }
}