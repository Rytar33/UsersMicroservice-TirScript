using System.Security.Cryptography;
using System.Text;

namespace Models.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Шифрует значение в формат SHA256
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetSha256(this string value) 
        => string.Concat(
            SHA256.Create()
                .ComputeHash(
                    Encoding.UTF8.GetBytes(value))
                .Select(b => b.ToString("x2")));

    /// <summary>
    /// Генерирует токен
    /// </summary>
    /// <param name="token"></param>
    /// <param name="sizeToken">Размер токена, дефолтное значение 6</param>
    /// <returns>Возвращает случайно сгенерированные символы в виде токена</returns>
    public static string GetGenerateToken(this string token, int sizeToken = 6)
    {
        var stringBuilder = new StringBuilder(token).Clear();
        while (sizeToken > 0)
        {
            stringBuilder.Append(new Random().Next(0x0030, 0x007A)); // Генерация от 0 до 9 и A-Z, a-z
            sizeToken--;
        }
        return stringBuilder.ToString();
    }
}