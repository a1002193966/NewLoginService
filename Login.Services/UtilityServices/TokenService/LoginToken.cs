using Login.Data.Interface;
using Login.Integration.Interface.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Login.Services.UtilityServices.TokenService
{
    public class LoginToken: ILoginToken
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginDbContext _loginDbContext;

        public LoginToken(IConfiguration configuration, ILoginDbContext loginDbContext)
        {
            _configuration = configuration;
            _loginDbContext = loginDbContext;
        }

        public async Task<string> GenerateToken(LoginCommand loginCommand)
        {
            try
            {
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                SigningCredentials credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
                Claim[] claim = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, loginCommand.Email),
                    new Claim(JwtRegisteredClaimNames.NameId, await getAccountID(loginCommand.Email)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                JwtSecurityToken token = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Issuer"],
                    claims: claim,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credential
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex) { throw; }


        }

        private async Task<string> getAccountID(string email)
        {
            return _loginDbContext.Account.Where(x => x.NormalizedEmail == email.ToUpper()).Select(x => x.Id.ToString()).FirstOrDefault();
                //"228D03E9-8CCF-4EF6-9AFA-11C4942B8012";
        }

    }
}
