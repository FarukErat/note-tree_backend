using Workout.Authentication.Interfaces;
using Workout.Notes.Interfaces;
using Workout.Authentication.Services;

namespace Workout.Extensions;

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