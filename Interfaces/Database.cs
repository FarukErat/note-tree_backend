using Workout.Models;

namespace Workout.Interfaces;

public interface IDBService
{
    Task CreateUserAsync(User user);
    Task<User?> FindByUsernameAsync(string userName);
    Task UpdatePasswordAsync(string username, Password password);
}