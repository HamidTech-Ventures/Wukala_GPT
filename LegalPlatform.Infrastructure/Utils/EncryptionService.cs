using System.Security.Cryptography;
using System.Text;
using LegalPlatform.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LegalPlatform.Infrastructure.Utils;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _keyBytes;

    public EncryptionService(IConfiguration configuration)
    {
        var key = configuration.GetSection("Crypto")["AesKey"]!;
        _keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
    }

    public (byte[] cipherBytes, byte[] iv) EncryptAes(byte[] plainBytes)
    {
        using var aes = Aes.Create();
        aes.Key = _keyBytes;
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return (cipher, aes.IV);
    }

    public byte[] DecryptAes(byte[] cipherBytes, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = _keyBytes;
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        return decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
    }

    public string EncryptString(string plainText)
    {
        var bytes = Encoding.UTF8.GetBytes(plainText);
        var (cipher, iv) = EncryptAes(bytes);
        return Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(cipher);
    }

    public string DecryptString(string cipherBase64)
    {
        var parts = cipherBase64.Split(':', 2);
        var iv = Convert.FromBase64String(parts[0]);
        var cipher = Convert.FromBase64String(parts[1]);
        var plainBytes = DecryptAes(cipher, iv);
        return Encoding.UTF8.GetString(plainBytes);
    }
}