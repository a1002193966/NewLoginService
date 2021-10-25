using System.Threading.Tasks;

namespace Login.Services.UtilityServices.PasswordService
{
    public interface ICryptoService
    {
        Task<Secret> EncryptAes(string password);
        Task<string> DecryptAes(Secret secret);
        Task<byte[]> EncryptAesWithKeyAndIV(string password, byte[] key, byte[] IV);
    }
}
