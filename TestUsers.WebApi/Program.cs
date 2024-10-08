using TestUsers.Services;
using TestUsers.Services.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using Microsoft.Extensions.Options;
using TestUsers.Services.Interfaces.Options;
using TestUsers.WebApi.Options;
using TestUsers.WebApi.Middlewares;

namespace TestUsers.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString, p => p.MigrationsAssembly("TestUsers.Data")));

        // Сервисы
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

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
