using Bogus;
using Models;
using Models.Extensions;

namespace Services.Tests;

/// <summary>
/// Сервис для генерации фейковых случайных данных
/// </summary>
public static class FakeDataService
{
    private static readonly Faker Faker = new("ru");

    /// <summary>
    /// Генерирует случайного пользователя
    /// </summary>
    /// <returns></returns>
    public static User GetGenerationUser() 
        => new(Faker.Internet.Email(), Faker.Name.FullName(), Faker.Internet.Password().GetSha256());
}