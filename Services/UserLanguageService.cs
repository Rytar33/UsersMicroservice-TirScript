using AutoMapper;
using Models;
using Models.Extensions;
using Services.Dtos;
using Services.Dtos.UserLanguages;
using Services.Dtos.Validators.UserLanguages;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class UserLanguageService(
    IMapper mapper,
    IUserLanguageRepository userLanguageRepository) : IUserLanguageService
{
    public async Task<List<UserLanguageItemResponse>> GetList(int userId, CancellationToken cancellationToken = default)
    {
        var userLanguages = await userLanguageRepository.GetListByExpression(ul => ul.UserId == userId, cancellationToken, ul => ul.Language);
        return userLanguages.Select(mapper.Map<UserLanguage, UserLanguageItemResponse>).ToList();
    }

    public async Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request, CancellationToken cancellationToken = default)
    {
        _ = new AddLanguageToUserValidator().ValidateWithErrors(request);
        var userLanguage = mapper.Map<AddLanguageToUser, UserLanguage>(request);
        await userLanguageRepository.CreateAsync(userLanguage, cancellationToken);
        await userLanguageRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> SaveUserLanguages(SaveUserLanguagesRequest request, CancellationToken cancellationToken = default)
    {
        _ = new SaveUserLanguagesRequestValidator().ValidateWithErrors(request);
        try
        {
            await userLanguageRepository.StartTransaction(cancellationToken);
            var newUserLanguages = mapper.Map<SaveUserLanguagesRequest, List<UserLanguage>>(request);

            var oldUserLanguages = await userLanguageRepository
                .GetListByExpression(ul => ul.UserId == request.UserId, cancellationToken);

            var oldUserLanguagesDeleting = oldUserLanguages.Where(userLanguage => !newUserLanguages.Any(nul =>
                userLanguage.UserId == nul.UserId
                && nul.LanguageId == userLanguage.LanguageId)).ToList();

            foreach (var userLanguage in oldUserLanguagesDeleting)
            {
                userLanguageRepository.Delete(userLanguage);
            }

            await userLanguageRepository.SaveChangesAsync(cancellationToken);

            foreach (var userLanguage in newUserLanguages)
            {
                if (!oldUserLanguages.Any(oul => oul.Id != 0 && userLanguage.Id == oul.Id))
                    await userLanguageRepository.CreateAsync(userLanguage, cancellationToken);
                else
                {
                    var oldUserLanguage =
                        await userLanguageRepository.GetByExpression(oul => 
                            oul.UserId == userLanguage.UserId 
                            && oul.LanguageId == userLanguage.LanguageId,
                            cancellationToken);
                    if (oldUserLanguage != null)
                        await userLanguageRepository.UpdateAsync(userLanguage, cancellationToken);
                    else
                        await userLanguageRepository.CreateAsync(userLanguage, cancellationToken);
                }
            }
            await userLanguageRepository.SaveChangesAsync(cancellationToken);
            await userLanguageRepository.CommitTransaction(cancellationToken);
            return new BaseResponse();
        }
        catch (Exception)
        {
            await userLanguageRepository.RollBackTransaction(cancellationToken); 
            throw;
        }
    }
}