using MailKit.Net.Smtp;
using MimeKit;
using TestUsers.Services.Exceptions;
using TestUsers.Data.Models;
using TestUsers.Services.Interfaces.Options;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using System.Data.Entity;

namespace TestUsers.Services;

public class EmailService(IEmailOptions options, DataContext db) : IEmailService
{
    public async Task SendEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await db.User.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User.Email)));
        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(options.NameCompany, options.Email));
        emailMessage.To.Add(new MailboxAddress(user.FullName, user.Email));
        emailMessage.Subject = user.FullName;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
        {
            Text = string.Concat($"Ваш код восстановления аккаунта: {user.RecoveryToken}.\nНе передавайте никому этот код.")
        };

        using var client = new SmtpClient();

        client.LocalDomain = options.Domain;
        await client.ConnectAsync(options.Host, int.Parse(options.Port), MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(options.Email, options.Password, cancellationToken);
        await client.SendAsync(emailMessage, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);

    }
}