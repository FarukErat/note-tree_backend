using Workout.Interfaces;
using Workout.Services;

namespace Workout.Extensions;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        // singleton
        services.AddSingleton<ConfigProvider>();
        services.AddSingleton<ICacheService, Redis>();
        services.AddSingleton<ICipher, Cipher>();

        // scoped
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IDBService, PostgreSql>();
    }
}