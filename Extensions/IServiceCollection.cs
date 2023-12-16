using NoteTree.Authentication.Interfaces;
using NoteTree.Notes.Interfaces;
using NoteTree.Authentication.Services;

namespace NoteTree.Extensions;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        // singleton
        services.AddSingleton<ConfigProvider>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<ICipher, Cipher>();
        services.AddSingleton<INoteDataManager, NoteDataManager>();

        // scoped
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserAuthDataManager, UserAuthDataManager>();
    }
}