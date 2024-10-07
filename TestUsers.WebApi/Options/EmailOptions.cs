using TestUsers.Services.Interfaces.Options;

namespace TestUsers.WebApi.Options;

public record EmailOptions : IEmailOptions
{
    public EmailOptions() {}

    public EmailOptions(
        string nameCompany,
        string email,
        string password,
        string host,
        string port,
        string domain)
    {
        NameCompany = nameCompany;
        Email = email;
        Password = password;
        Host = host;
        Port = port;
        Domain = domain;
    }

    public string NameCompany { get; init; }

    public string Email { get; init; }

    public string Password { get; init; }

    public string Host { get; init; }

    public string Port { get; init; }

    public string Domain { get; init; }
}