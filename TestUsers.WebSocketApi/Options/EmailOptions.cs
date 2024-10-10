using TestUsers.Services.Interfaces.Options;

namespace TestUsers.WebSocketApi.Options;

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

    public string NameCompany { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string Host { get; init; } = string.Empty;

    public string Port { get; init; } = string.Empty;

    public string Domain { get; init; } = string.Empty;
}