﻿using Bogus;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.UserContacts;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Exceptions;
using Xunit;

namespace TestUsers.Services.Tests;

/// <summary>
/// Юнит тесты для сервиса контактов пользователя
/// </summary>
public class UserContactServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly DataContext _dataContext;
    private readonly Faker _faker;
    private readonly IUserContactService _userContactService;
    public UserContactServiceTests()
    {
        _faker = new Faker("ru");
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(UserContactServiceTests))
            .Options;
        _dataContext = new DataContext(_dbContextOptions);
        _userContactService = new UserContactService(_dataContext);
    }

    /// <summary>
    /// Тест проверяет, возвращаются ли контакты пользователя в случае, если пользователь существует
    /// </summary>
    [Fact]
    public async Task GetContacts_ShouldReturnMappedContacts_WhenUserExists()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dataContext.User.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        var contact = new UserContact(FakeDataService.GetUniqueName(_faker.Lorem.Word()), FakeDataService.GetUniqueName(_faker.Phone.PhoneNumber()), user.Id);
        await _dataContext.UserContact.AddAsync(contact);
        await _dataContext.SaveChangesAsync();

        // Act
        var result = await _userContactService.GetContacts(user.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(contact.Id, result[0].Id);
    }

    /// <summary>
    /// Тест проверяет, выбрасывается ли исключение, если пользователь не найден
    /// </summary>
    [Fact]
    public async Task GetContacts_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var userId = 100;
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userContactService.GetContacts(userId));
    }

    /// <summary>
    /// Тест проверяет, создаются ли новые контакты пользователя, если они не существовали ранее
    /// </summary>
    [Fact]
    public async Task SaveContacts_ShouldCreateNewContactsAndDeleteNotPresent_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dataContext.User.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        var contacts = new List<UserContact>()
        {
            new(FakeDataService.GetUniqueName(_faker.Lorem.Word()), FakeDataService.GetUniqueName(_faker.Phone.PhoneNumber()), user.Id),
            new(FakeDataService.GetUniqueName(_faker.Lorem.Word()), FakeDataService.GetUniqueName(_faker.Internet.Url()), user.Id),
            new(FakeDataService.GetUniqueName(_faker.Lorem.Word()), FakeDataService.GetUniqueName(_faker.Internet.Url()), user.Id)
        };
        await _dataContext.UserContact.AddRangeAsync(contacts);
        await _dataContext.SaveChangesAsync();

        var request = new UserContactsSaveRequest(
            user.Id, 
            [
                new UserContactItem(null, FakeDataService.GetUniqueName(_faker.Lorem.Word()), FakeDataService.GetUniqueName(_faker.Internet.Url())),
                new UserContactItem(contacts[0].Id, contacts[0].Name, contacts[0].Value),
                new UserContactItem(contacts[1].Id, contacts[1].Name, contacts[1].Value),
            ]);

        // Act
        await _userContactService.SaveContacts(request);

        // Assert
        Assert.False(await _dataContext.UserContact.AnyAsync(uc => uc.Id == contacts[2].Id));
        Assert.True(await _dataContext.UserContact.AnyAsync(uc => uc.Name == request.Contacts[0].Name));
    }
}