namespace Workout.Interfaces;

public interface ICipher
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}