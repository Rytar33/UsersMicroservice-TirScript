using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Exceptions;
using Models.Validations;
using Models;
using Services.Interfaces.Options;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class EmailService(IOptions<IEmailOptions> options, IUserRepository userRepository) : IEmailService
{
    private readonly IEmailOptions _emailOptions = options.Value;

    public async Task SendEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByExpression(u => u.Email == email, cancellationToken);
        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User.Email)));

        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(_emailOptions.NameCompany, _emailOptions.Email));
        emailMessage.To.Add(new MailboxAddress(user.FullName, user.Email));
        emailMessage.Subject = user.FullName;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
        {
            Text = string.Concat($"Ваш код восстановления аккаунта: {user.RecoveryToken}.\nНе передавайте никому этот код.")
        };

        using var client = new SmtpClient();

        client.LocalDomain = _emailOptions.Domain;
        await client.ConnectAsync(_emailOptions.Host, int.Parse(_emailOptions.Port), MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_emailOptions.Email, _emailOptions.Password, cancellationToken);
        await client.SendAsync(emailMessage, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);

    }
}