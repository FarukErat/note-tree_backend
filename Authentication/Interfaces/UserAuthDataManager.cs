using Workout.Authentication.Models;

namespace Workout.Authentication.Interfaces;

public interface IUserAuthDataManager
{
    Task CreateUserAsync(User user);
    Task<User?> FindByUsernameAsync(string userName);
    Task UpdatePasswordAsync(string username, string passwordHash, string passwordAlgo);
    Task SetNoteRecordIdAsync(string userId, string noteRecordId);
}