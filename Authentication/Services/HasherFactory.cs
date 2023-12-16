using NoteTree.Constants;
using NoteTree.Authentication.Interfaces;

namespace NoteTree.Authentication.Services;

public class HasherFactory
{
    public static IPasswordManager GetHasherByAlgorithm(string algorithm)
    {
        switch (algorithm.ToLower())
        {
            case HashAlgorithms.Argon2id:
                return new Argon2idHasher();
            case HashAlgorithms.Bcrypt:
                return new BcryptHasher();
            default:
                throw new ArgumentException("Unsupported hashing algorithm.", nameof(algorithm));
        }
    }
}