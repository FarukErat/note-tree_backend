using Workout.Models;

namespace Workout.Interfaces;

public interface IDBService
{
    Task CreateUserAsync(AppUser user);
    Task<AppUser?> FindByUsernameAsync(string userName);
    Task UpdatePasswordAsync(string username, Password password);
}