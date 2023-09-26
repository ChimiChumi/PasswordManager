using System.Security.Cryptography;
using System.Text;
using Fernet;

public class EncryptedType
{
    private const int KeySize = 256; // Use 256 bits (32 bytes) key size for AES
    private const int SaltSize = 16; // Salt size for PBKDF2

    public static Encrypt(string secret)
    {
        using var hashing = SHA256.Create();
        byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(secret));
        string key = Base64Url.Encode(keyHash);
        string message = Base64Url.Encode(Encoding.Unicode.GetBytes(secret));
        string encryptedSecret = Fernet.Encrypt(key, message);
        return new EncryptedType(key, encryptedSecret);
    }

    public static Decrypt(string key, string encryptedSecret)
    {
        string decodedKey = Base64Url.Decode(key);
        string decodedSecret = Fernet.Decrypt(decodedKey, encryptedSecret);
        string secret = Encoding.Unicode.GetString(Base64Url.Decode(decodedSecret));
        return new EncryptedType(key, secret);
    }
}
