using AutoMapper;
using MimeKit;
using Models;
using Models.Exceptions;
using Models.Validations;
using Services.Dtos;
using Services.Dtos.Pages;
using Services.Dtos.Users;
using Services.Dtos.Users.Recoveries;
using Services.Interfaces;
using MailKit.Net.Smtp;
using Models.Extensions;
using Services.Dtos.Validators;

namespace Services;

public class UserService(
    IUserRepository userRepository,
    IMapper mapper) : IUserService
{
    public async Task<UsersListResponse> GetList(UsersListRequest request, CancellationToken cancellationToken = default)
    {
        _ = new UsersListRequestValidator().ValidateWithErrors(request);
        var users = await userRepository.GetListByExpression(cancellationToken: cancellationToken);
        var usersForConditions = users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            usersForConditions = usersForConditions
                .Where(x => x.FullName.Contains(request.Search) || x.Email.Contains(request.Search));

        if (request.Status != null)
            usersForConditions = usersForConditions.Where(x => x.Status == request.Status);

        var countUsers = usersForConditions.Count();

        usersForConditions = usersForConditions
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
            .Take(request.Page.PageSize);

        var usersItems = usersForConditions.Select(u => mapper.Map<User, UsersListItem>(u)).ToList();

        return new UsersListResponse(usersItems, new PageResponse(request.Page.Page, request.Page.PageSize, countUsers));
    }

    public async Task<UserDetailResponse> GetDetail(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByExpression(u => u.Id == userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        return mapper.Map<User, UserDetailResponse>(user);
    }

    public async Task<BaseResponse> Create(UserCreateRequest request, CancellationToken cancellationToken = default)
    {
        _ = new UserCreateRequestValidator().ValidateWithErrors(request);
        var user = mapper.Map<UserCreateRequest, User>(request);
        await userRepository.CreateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Edit(UserEditRequest request, CancellationToken cancellationToken = default)
    {
        _ = new UserEditRequestValidator().ValidateWithErrors(request);
        var user = await userRepository.GetByExpression(u => u.Id == request.Id, cancellationToken);
        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        user.FullName = request.FullName;
        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByExpression(u => u.Id == userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        userRepository.Delete(user);
        await userRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> RecoveryStart(RecoveryStartRequest request, CancellationToken cancellationToken = default)
    {
        _ = new RecoveryStartRequestValidator().ValidateWithErrors(request);

        var user = await userRepository.GetByExpression(u => u.Email == request.Email, cancellationToken);

        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));

        if (request.RequestCode == null)
        {
            user.RecoveryToken = user.RecoveryToken.GetGenerateToken();
            await userRepository.UpdateAsync(user, cancellationToken);
            await userRepository.SaveChangesAsync(cancellationToken);
            await SendIntoEmail(user.Email, cancellationToken);
        }
        else
        {
            if (user.RecoveryToken != request.RequestCode)
                throw new ArgumentException(string.Format(ErrorMessages.NotCoincideError, nameof(User.RecoveryToken)));
        }
        

        return new BaseResponse();
    }

    public async Task<BaseResponse> RecoveryEnd(RecoveryEndRequest request, CancellationToken cancellationToken = default)
    {
        _ = new RecoveryEndRequestValidator().ValidateWithErrors(request);

        var user = await userRepository.GetByExpression(u => u.Email == request.Email, cancellationToken);

        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));

        if (user.RecoveryToken != request.RecoveryTokenRequest)
            throw new ArgumentException(string.Format(ErrorMessages.NotCoincideError, nameof(User.RecoveryToken)));

        user.PasswordHash = request.NewPassword.GetSha256();
        user.RecoveryToken = null;
        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }

    /// <summary>
    /// Отправка по почте письма ассинхронно
    /// </summary>
    /// <param name="email">Электронная почта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task SendIntoEmail(string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByExpression(u => u.Email == email, cancellationToken);
        if (user == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User.Email)));

        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("TestCompany", "anyEmailCompany"));
        emailMessage.To.Add(new MailboxAddress(user.FullName, user.Email));
        emailMessage.Subject = user.FullName;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
        {
            Text = string.Concat($"Ваш код восстановления аккаунта: {user.RecoveryToken}.\nНе передавайте никому этот код.")
        };

        using var client = new SmtpClient();
        
        client.LocalDomain = "localhost";
        await client.ConnectAsync("smtp.mail.ru", 587, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync("anyEmailCompany", "password", cancellationToken);
        await client.SendAsync(emailMessage, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);

    }
}