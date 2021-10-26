using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Data.Interface;
using Login.DomainModel;
using Login.Integration.Interface.Commands;
using Login.Integration.Interface.Responses;
using Login.Services.UtilityServices.PasswordService;
using Login.Services.UtilityServices.TokenService;

namespace Login.Services.CommandHandlers
{
    public class LoginHandler : Core.RequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ILoginDbContext _loginDbContext;
        private readonly ICryptoService _cryptoService;
        private readonly ILoginToken _loginToken;
        public LoginHandler(ILoginDbContext loginDbContext, ICryptoService cryptoService, ILoginToken loginToken)
        {
            _loginDbContext = loginDbContext;
            _cryptoService = cryptoService;
            _loginToken = loginToken;
        }

        protected override async Task<LoginResponse> HandleRequest(LoginCommand request, CancellationToken ct)
        {

            var acc = _loginDbContext.Account.Where(x => x.NormalizedEmail == request.Email.ToUpper()).Select(  x => new{x.Cipher, x.Key,  x.IV }).FirstOrDefault();
            if (acc == null)
                throw new ArgumentException($"Invalid  Email.");

            Secret secret = new()
            {
                Cipher = acc.Cipher,
                Key = acc.Key,
                IV = acc.IV

            };

            if (!await Login(request, secret))
                throw new ArgumentException($"Incorrect password for account: {request.Email}.");

            var tokenString = await _loginToken.GenerateToken(request);

            LoginResponse response = new()
            {
                Token = tokenString,
                Status = true
            };
            return response;
        }


            private async Task<bool> Login(LoginCommand request, Secret secret)
        {
            // byte[] passwordHash = await getPasswordHash(request.Email, request.Password

            var passwd = _cryptoService.DecryptAes(secret);
            if (await passwd == request.Password)
            {
                return true;
            }
            return false;

        }
    }
}
