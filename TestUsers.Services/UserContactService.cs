using TestUsers.Data.Models;
using TestUsers.Services.Dtos.UserContacts;
using TestUsers.Services.Dtos.Validators.UserContacts;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using TestUsers.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace TestUsers.Services;

public class UserContactService(DataContext db) : IUserContactService
{
    public async Task<List<UserContactItem>> GetContacts(int userId, CancellationToken cancellationToken = default)
    {
        if (!await db.User.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        return await db.UserContact
            .Where(uc => uc.UserId == userId)
            .Select(uc => 
            new UserContactItem(uc.Id, uc.Name, uc.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveContacts(
        UserContactsSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        await new UserContactsSaveRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        // Получаем существующие контакты пользователя
        var oldUserContacts = await db.UserContact
            .Where(uc => uc.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        // Определяем контакты, которые нужно удалить
        var deletingUserContacts = oldUserContacts
            .Where(ouc => !request.Contacts.Any(c => c.Id.HasValue && c.Id == ouc.Id))
            .ToList();

        if (deletingUserContacts.Count != 0)
            db.UserContact.RemoveRange(deletingUserContacts);

        // Создаем новые контакты
        var newUserContacts = request.Contacts
            .Where(nuc => nuc.Id == null) // Добавляем только новые контакты (с null Id)
            .Select(nuc => new UserContact(nuc.Name, nuc.Value, request.UserId))
            .ToList();

        // Проверяем на совпадения уже существующих контактов в базе данных
        var existingContacts = await db.UserContact
            .Where(uc => uc.UserId == request.UserId)
            .Select(uc => uc.Name)
            .ToListAsync(cancellationToken);

        if (newUserContacts.Any(nuc => existingContacts.Contains(nuc.Name)))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(UserContact.Name), nameof(UserContact)));

        if (newUserContacts.Count != 0)
            await db.UserContact.AddRangeAsync(newUserContacts, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}