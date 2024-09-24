namespace Services.Interfaces.Options;

public interface IEmailOptions 
{
    string NameCompany { get; init; }

    string Email { get; init; }

    string Password { get; init; }

    string Port { get; init; }

    string Host { get; init; }

    string Domain { get; init; }
}