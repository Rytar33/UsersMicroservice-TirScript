using AutoMapper;
using Models;
using Models.Exceptions;
using Models.Extensions;
using Models.Validations;
using Services.Dtos.UserContacts;
using Services.Dtos.Validators.UserContacts;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class UserContactService(
    IMapper mapper,
    IUserContactRepository userContactRepository,
    IUserRepository userRepository) : IUserContactService
{
    public async Task<List<UserContactItem>> GetContacts(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByExpression(
            u => u.Id == userId,
            cancellationToken,
            u => u.Contacts);
        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        return user.Contacts.Select(mapper.Map<UserContact, UserContactItem>).ToList();
    }

    public async Task SaveContacts(
        UserContactsSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = new UserContactsSaveRequestValidator().ValidateWithErrors(request);
        
        try
        {
            await userContactRepository.StartTransaction(cancellationToken);

            var oldUserContacts = await userContactRepository.GetListByExpression(uc => uc.UserId == request.UserId, cancellationToken);
            var newUserContacts = 
                request.Contacts
                    .Select(nuc => nuc.Id != null 
                        ? new UserContact(nuc.Name, nuc.Value, request.UserId){ Id = (int)nuc.Id } 
                        : new UserContact(nuc.Name, nuc.Value, request.UserId)).ToList(); 
            oldUserContacts.ForEach(ouc => 
            {
                if(newUserContacts.All(uc => uc.Id != ouc.Id))
                    userContactRepository.Delete(ouc);
            });
            foreach (var userContact in newUserContacts)
            {
                if (userContact.Id == 0)
                    await userContactRepository.CreateAsync(userContact, cancellationToken);
                else
                    await userContactRepository.UpdateAsync(userContact, cancellationToken);
            }
            await userContactRepository.SaveChangesAsync(cancellationToken);

            await userContactRepository.CommitTransaction(cancellationToken);
        }
        catch (Exception)
        {
            await userContactRepository.RollBackTransaction(cancellationToken);
            throw;
        }
    }
}