using Login.Integration.Interface.Commands;
using System.Threading.Tasks;

namespace Login.Services.UtilityServices.TokenService
{
    public interface ILoginToken
    {
        Task<string> GenerateToken(LoginCommand loginInput);
    }
}
