namespace Services.Interfaces.Services;

public interface IEmailService
{
    /// <summary>
    /// Отправка по почте письма ассинхронно
    /// </summary>
    /// <param name="email">Электронная почта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task SendEmailAsync(string email, CancellationToken cancellationToken = default);
}