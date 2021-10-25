using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Login.Services.UtilityServices.PasswordService
{
    public class CryptoService : ICryptoService
    {
        public async Task<Secret> EncryptAes(string password)
        {
            Secret secret = new();
            using (Aes aes = Aes.Create())
            {
                secret.Key = aes.Key;
                secret.IV = aes.IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream ms = new();
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                {
                    await sw.WriteAsync(password);
                }
                secret.Cipher = ms.ToArray();
            }
            return secret;
        }

        public async Task<string> DecryptAes(Secret secret)
        {
            string password = null;
            using (Aes aes = Aes.Create())
            {
                aes.Key = secret.Key;
                aes.IV = secret.IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using MemoryStream ms = new(secret.Cipher);
                using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                password = await sr.ReadToEndAsync();
            }
            return password;
        }

        public async Task<byte[]> EncryptAesWithKeyAndIV(string password, byte[] key, byte[] IV)
        {
            byte[] cipher;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream ms = new();
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                {
                    await sw.WriteAsync(password);
                }
                cipher = ms.ToArray();
            }
            return cipher;
        }

    }
}
