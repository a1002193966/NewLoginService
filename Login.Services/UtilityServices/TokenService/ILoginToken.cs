
namespace Login.Services.UtilityServices.TokenService
{
    public interface ILoginToken
    {
        string GenerateToken(string id, string email);
    }
}
