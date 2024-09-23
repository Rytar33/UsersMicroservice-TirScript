using AutoMapper;
using Models;
using Models.Extensions;
using Services.Dtos.Users;

namespace Services.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        #region Responses Map

        CreateMap<User, UserDetailResponse>();

        CreateMap<User, UsersListItem>();

        #endregion

        #region Requests Map

        CreateMap<UserCreateRequest, User>()
            .ConstructUsing(u => new User(u.Email, u.FullName, u.Password.GetSha256()));

        #endregion
    }
}