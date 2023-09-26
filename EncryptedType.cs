using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
   /* class EncryptedType
    {
        Key  // user email
    ...Secret  // data to be encrypted

    EncryptedType Encrypt()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(Key));
            string key = Base64UrlEncoder.Encode(keyHash);
            string message = Base64UrlEncoder.Encode(Encoding.Unicode.GetBytes(Secret));
            return new(Key, Fernet.Encrypt(key, message));
        }

        EncryptedType Decrypt()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(Key));
            string key = Base64UrlEncoder.Encode(keyHash);
            string encodedSecret = Fernet.Decrypt(key, Secret);
            string message = Encoding.Unicode.GetString(Base64UrlEncoder.DecodeBytes(encodedSecret));
            return new(Key, message);
        }
    }*/
}
