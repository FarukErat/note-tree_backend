using System.Security.Cryptography;
using System.Text;
using Workout.Authentication.Interfaces;

namespace Workout.Authentication.Services;

public class Cipher : ICipher
{
    private readonly byte[] _key;

    public Cipher(ConfigProvider configProvider)
    {
        byte[] originalKey = Encoding.UTF8.GetBytes(configProvider.CipherKey);
        _key = DeriveValidKey(originalKey, 32);
    }

    private static byte[] DeriveValidKey(byte[] originalKey, int keySize)
    {
        byte[] hash = SHA256.HashData(originalKey);
        byte[] derivedKey = new byte[keySize];
        Array.Copy(hash, derivedKey, Math.Min(hash.Length, derivedKey.Length));
        return derivedKey;
    }

    public string Encrypt(string plainText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = _key;
        aesAlg.GenerateIV();

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray());
    }

    public string Decrypt(string cipherText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = _key;

        byte[] iv = new byte[16];
        Array.Copy(Convert.FromBase64String(cipherText), 0, iv, 0, 16);
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText).Skip(16).ToArray());
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}
