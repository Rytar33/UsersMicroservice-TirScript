using Users.BaseIdentity;

namespace TestUsers.Services.Dtos.UserIdentities;

public class CurrentWsUser : IUser<int>
{
    public int Id { get; set; }

    public bool IsAuthorized { get; set; }

    public ICollection<string>? Roles { get; set; }
}
