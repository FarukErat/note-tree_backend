using Workout.Data;
using Microsoft.EntityFrameworkCore;
using Workout.Interfaces;
using Workout.Models;

namespace Workout.Services;

public class PostgreSql : IDBService
{
    private readonly AppDbContext _workoutContext;

    public PostgreSql(AppDbContext workoutContext)
    {
        _workoutContext = workoutContext ?? throw new ArgumentNullException(nameof(workoutContext));
    }

    public async Task CreateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await _workoutContext.Users.AddAsync(user);
        await _workoutContext.SaveChangesAsync();
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        User? user = await _workoutContext.Users
            .FirstOrDefaultAsync(u => u.UserName == username);

        return user;
    }

    public async Task UpdatePasswordAsync(string username, string passwordHash, string passwordAlgo)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        User? user = await _workoutContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user != null)
        {
            user.PasswordHash = passwordHash;
            user.PasswordAlgo = passwordAlgo;
            await _workoutContext.SaveChangesAsync();
        }
    }
}