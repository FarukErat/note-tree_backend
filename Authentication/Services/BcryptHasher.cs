using NoteTree.Authentication.Interfaces;

namespace NoteTree.Authentication.Services;

public class BcryptHasher : IPasswordManager
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}