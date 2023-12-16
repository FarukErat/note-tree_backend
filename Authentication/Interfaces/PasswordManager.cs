namespace NoteTree.Authentication.Interfaces;

public interface IPasswordManager
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}