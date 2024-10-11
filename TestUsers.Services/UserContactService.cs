using TestUsers.Data.Models;
using TestUsers.Services.Dtos.UserContacts;
using TestUsers.Services.Dtos.Validators.UserContacts;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using TestUsers.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace TestUsers.Services;

public class UserContactService(DataContext _db) : IUserContactService
{
    public async Task<List<UserContactItem>> GetContacts(int userId, CancellationToken cancellationToken = default)
    {
        if (!await _db.User.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        return await _db.UserContact
            .Where(uc => uc.UserId == userId)
            .Select(uc => 
            new UserContactItem(uc.Id, uc.Name, uc.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveContacts(
        UserContactsSaveRequest request,
        Guid? sessionId = null,
        CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (request.UserId != userSession.UserId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }

        await new UserContactsSaveRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        // Получаем существующие контакты пользователя
        var oldUserContacts = await _db.UserContact
            .Where(uc => uc.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        // Определяем контакты, которые нужно удалить
        var deletingUserContacts = oldUserContacts
            .Where(ouc => !request.Contacts.Any(c => c.Id.HasValue && c.Id == ouc.Id))
            .ToList();

        if (deletingUserContacts.Count != 0)
            _db.UserContact.RemoveRange(deletingUserContacts);

        // Создаем новые контакты
        var newUserContacts = request.Contacts
            .Where(nuc => nuc.Id == null) // Добавляем только новые контакты (с null Id)
            .Select(nuc => new UserContact(nuc.Name, nuc.Value, request.UserId))
            .ToList();

        // Проверяем на совпадения уже существующих контактов в базе данных
        var existingContacts = oldUserContacts.Select(uc => uc.Name);

        if (newUserContacts.Any(nuc => existingContacts.Contains(nuc.Name)))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(UserContact.Name), nameof(UserContact)));

        if (newUserContacts.Count > 0)
            await _db.UserContact.AddRangeAsync(newUserContacts, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}