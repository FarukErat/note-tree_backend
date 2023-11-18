using Workout.Models;

namespace Workout.Interfaces;

public interface ICacheService
{
    // session
    Task SaveUserAsync(User user, HttpContext context);
    Task<Session?> GetSessionByIdAsync(string sessionId);
    Task UpdateSessionByIdAsync(string sessionId, Session session);
    Task DeleteSessionByIdAsync(string sessionId);
}