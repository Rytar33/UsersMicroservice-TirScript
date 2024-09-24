using Services.Interfaces.Options;

namespace Services.Options;

public record EmailOptions(
    string NameCompany,
    string Email,
    string Password,
    string Host,
    string Port,
    string Domain) : IEmailOptions;