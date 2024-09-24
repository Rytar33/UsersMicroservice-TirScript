using AutoMapper;
using Models;
using Services.Dtos.UserContacts;

namespace Services.MappingProfiles;

public class UserContactProfile : Profile
{
    public UserContactProfile()
    {
        CreateMap<UserContact, UserContactItem>();
    }
}