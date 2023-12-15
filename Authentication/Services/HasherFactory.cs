using Workout.Constants;
using Workout.Authentication.Interfaces;

namespace Workout.Authentication.Services;

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