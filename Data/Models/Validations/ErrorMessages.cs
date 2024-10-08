﻿namespace Models.Validations;

public static class ErrorMessages
{
    public static string NotFoundError = "{0} не был найден";

    public static string EmptyError = "{0} не должен быть пуст";

    public static string RegexError = "Неправильный формат данных";

    public static string FutureError = "{0} не должен быть в будущем";

    public static string CoincideError = "С таким {0} уже существует {1}";

    public static string NotCoincideError = "{0} не совпадает";

    public static string LessThanError = "{0} не должно быть меньше {1}";
}