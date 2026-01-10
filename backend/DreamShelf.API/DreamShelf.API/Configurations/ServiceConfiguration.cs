using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Configurations.Options;
using DreamShelf.API.Persistence;
using DreamShelf.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDatabase(configuration);
        services.ConfigureOptions(configuration);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IDocumentParserService, DocumentParserService>();
        services.AddScoped<IGenresService, GenresService>();
        services.AddScoped<IChapterService, ChapterService>();
        services.AddScoped<IAdminPanelService, AdminPanelService>();
        services.AddScoped<IAuthorService, AuthorService>();
        
        services.AddSingleton<IAuthorSearchService, AuthorSearchService>();
        services.AddSingleton<IBookSearchService, BookSearchService>();
        return services;
    }

    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AuthorizationOptions>(configuration.GetSection(AuthorizationOptions.SectionName));
        return services;
    }
    
    
    private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Register database-related services here
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
}