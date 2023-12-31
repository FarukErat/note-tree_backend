using NoteTree.Authentication.Data;
using Microsoft.EntityFrameworkCore;
using NoteTree.Authentication.Interfaces;
using NoteTree.Authentication.Models;

namespace NoteTree.Authentication.Services;

public class UserAuthDataManager : IUserAuthDataManager
{
    private readonly AppDbContext _noteTreeContext;

    public UserAuthDataManager(AppDbContext noteTreeContext)
    {
        _noteTreeContext = noteTreeContext ?? throw new ArgumentNullException(nameof(noteTreeContext));
    }

    public async Task CreateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await _noteTreeContext.Users.AddAsync(user);
        await _noteTreeContext.SaveChangesAsync();
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        User? user = await _noteTreeContext.Users
            .FirstOrDefaultAsync(u => u.UserName == username);

        return user;
    }

    public async Task SetNoteRecordIdAsync(string userId, string noteRecordId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(noteRecordId))
        {
            throw new ArgumentNullException(nameof(noteRecordId));
        }

        User? user = await _noteTreeContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            user.NoteRecordId = noteRecordId;
            await _noteTreeContext.SaveChangesAsync();
        }
    }

    public async Task UpdatePasswordAsync(string username, string passwordHash, string passwordAlgo)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        User? user = await _noteTreeContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user != null)
        {
            user.PasswordHash = passwordHash;
            user.PasswordAlgo = passwordAlgo;
            await _noteTreeContext.SaveChangesAsync();
        }
    }
}