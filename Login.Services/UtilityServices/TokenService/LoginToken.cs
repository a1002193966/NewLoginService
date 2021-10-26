using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Login.Services.UtilityServices.TokenService
{
    public class LoginToken: ILoginToken
    {
        private readonly IConfiguration _configuration;

        public LoginToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string id, string email)
        {
            try
            {
                SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                SigningCredentials credential = new(securityKey, SecurityAlgorithms.HmacSha512);
                Claim[] claim = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.NameId, id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                JwtSecurityToken token = new
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
    }
}
