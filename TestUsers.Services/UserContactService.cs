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
        var contacts = db.UserContact.Where(uc => uc.UserId == userId)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        return await contacts
            .Select(uc => 
            new UserContactItem(uc.Id, uc.Name, uc.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveContacts(
        UserContactsSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        await new UserContactsSaveRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var oldUserContacts = await db.UserContact.Where(uc => uc.UserId == request.UserId).ToListAsync(cancellationToken);

        var deletingUserContacts = oldUserContacts.Where(ouc => !request.Contacts.Any(c => c.Id.HasValue && c.Id == ouc.Id)).ToList();

        if (deletingUserContacts.Count != 0)
            db.UserContact.RemoveRange(deletingUserContacts);

        var newUserContacts = request.Contacts
            .Where(nuc => nuc.Id != null)
            .Select(nuc => new UserContact(nuc.Name, nuc.Value, request.UserId))
            .ToList();

        if (await db.UserContact.AnyAsync(uc => newUserContacts.Any(nuc => nuc.UserId == uc.UserId && nuc.Name == uc.Name), cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(UserContact.Name), nameof(UserContact)));

        if (newUserContacts.Count != 0)
            await db.UserContact.AddRangeAsync(newUserContacts, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}