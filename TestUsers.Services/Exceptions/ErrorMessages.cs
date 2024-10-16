﻿namespace TestUsers.Services.Exceptions;

public static class ErrorMessages
{
    public static string NotFoundError = "{0} не был найден";

    public static string EmptyError = "{0} не должен быть пуст";

    public static string RegexError = "Неправильный формат данных";

    public static string FutureError = "{0} не должен быть в будущем";

    public static string PastError = "{0} не должен быть позднее чем {1}";

    public static string CoincideError = "С таким {0} уже существует {1}";

    public static string NotCoincideError = "{0} не совпадает";

    public static string LessThanError = "{0} не должно быть меньше {1}";

    public static string GreaterThanError = "{0} не должно быть больше {1}";

    public static string UnAuthError = "Авторизуйтесь чтобы совершить данное действие";

    public static string ForbiddenError = "У вас недостаточно прав, чтобы сделать это действие";

    public static string YouAuthError = "Вы уже авторизованны, выйдите из системы и повторите попытку";

    public static string YouAreBannedError = "Ваш аккаунт заблокирован";

    public static string SaveRequestError = "Не удалось сохранить {0}, возможно вы не правильно указали параметры";
}