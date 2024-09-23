using FluentValidation;

namespace Models.Extensions;

public static class ValidateExtensions
{
    /// <summary>
    /// Валлидирует значение объекта, если там будет не правильно, выдаст исключение со списком ошибок
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="validator"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public static T ValidateWithErrors<T>(this IValidator<T> validator, T value)
    {
        var validationResult = validator.Validate(value);
        return validationResult.IsValid
            ? value
            : throw new ValidationException(validationResult.Errors);
    }
}