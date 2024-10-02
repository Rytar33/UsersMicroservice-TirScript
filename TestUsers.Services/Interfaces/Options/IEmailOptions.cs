namespace TestUsers.Services.Interfaces.Options;

/// <summary>
/// Интерфейс опций для конфигурации электронной почты
/// </summary>
public interface IEmailOptions 
{
    string NameCompany { get; init; }

    string Email { get; init; }

    string Password { get; init; }

    string Port { get; init; }

    string Host { get; init; }

    string Domain { get; init; }
}