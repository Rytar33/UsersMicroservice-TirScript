using TestUsers.Services.Interfaces.Options;

namespace TestUsers.Services.Options;

public record EmailOptions(
    string NameCompany,
    string Email,
    string Password,
    string Host,
    string Port,
    string Domain) : IEmailOptions;