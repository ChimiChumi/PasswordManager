#pragma warning disable
using System.Security.Cryptography;
using System.Text;

public class EncryptedType
{
    private static byte[] GenerateKey(string user)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] userBytes = Encoding.UTF8.GetBytes(user);
            return sha256.ComputeHash(userBytes);
        }
    }

    public string Encrypt(string secret, string user)
    {
        byte[] key = GenerateKey(user);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.GenerateIV();

            // Create an encryptor to perform the stream transform
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Write the IV to the beginning of the stream
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(secret);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string Decrypt(string secret, string user)
    {
        byte[] key = GenerateKey(user);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;

            byte[] iv = new byte[aesAlg.BlockSize / 8];

            // Read the IV from the beginning of the encrypted data
            byte[] encryptedBytes = Convert.FromBase64String(secret);
            Array.Copy(encryptedBytes, iv, iv.Length);

            aesAlg.IV = iv;

            // Create a decryptor to perform the stream transform
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        string decryptedPassword = srDecrypt.ReadToEnd();
                        return decryptedPassword;
                    }
                }
            }
        }
    }
}
