using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using TestUsers.Data;
using TestUsers.Services;
using TestUsers.Services.Dtos.UserIdentities;
using TestUsers.Services.Interfaces.Options;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Middlewares;
using TestUsers.WebSocketApi.Options;
using Tirscript.Logger;
using Tirscript.Logger.LoggersImplementation;
using Users.IIdentityService;
using WebSocketControllers.Core.ExtensionsCore;

namespace TestUsers.WebSocketApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString, p => p.MigrationsAssembly("TestUsers.Data")));
        
        builder.Services.AddWebSocket<CurrentWsUser, int>(builder.Configuration);
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUsersIdentityService<CurrentWsUser, int>, UserIdentityService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<INewsService, NewsService>();
        builder.Services.AddScoped<IProductCategoryParametersService, ProductCategoryParametersService>();
        builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserContactService, UserContactService>();
        builder.Services.AddScoped<IUserLanguageService, UserLanguageService>();
        builder.Services.AddScoped<IUserSaveFilterService, UserSaveFilterService>();
        builder.Services.AddScoped<IUserService, UserService>();

        // Настраиваем IEmailOptions с привязкой к appsettings.json
        builder.Services.Configure<EmailOptions>(
            builder.Configuration.GetSection(nameof(EmailOptions)));

        // Регистрируем IEmailOptions для использования в сервисах
        builder.Services.AddSingleton<IEmailOptions>(sp =>
            sp.GetRequiredService<IOptions<EmailOptions>>().Value);


        builder.Services.AddTransient<ILoggerService, DefaultLoggerService>(provider => 
            new DefaultLoggerService(
                LoggerFactory
                    .Create(conf => conf.AddConsole())
                    .CreateLogger(typeof(Logger))));

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseMiddleware<WebSocketExceptionHandlingMiddleware>();

        app.UseWebSocket<CurrentWsUser, int>();

        app.MapControllers();

        app.Run();
    }
}
