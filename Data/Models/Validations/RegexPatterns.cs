using System.Text.RegularExpressions;

namespace Models.Validations;

public static class RegexPatterns
{
    /// <summary>
    /// Реджекс паттерн для электронных почт
    /// </summary>
    public static Regex EmailPattern = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

    /// <summary>
    /// Реджекс паттерн для ФИО
    /// </summary>
    public static Regex FullNamePattern = new(@"^[а-яА-ЯёЁa-zA-Z]{1,50}\s[а-яА-ЯёЁa-zA-Z]{1,50}(?:\s[а-яА-ЯёЁa-zA-Z]{0,50})?$");
}