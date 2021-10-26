using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Data.Interface;
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
            LoginResponse response = new();

            if (TryGetAccountInfoByEmail(request.Email, out var id, out var recordedSecret))
            {
                var isPasswordMatch = await TryLogin(request, recordedSecret);
                if (!isPasswordMatch)
                    throw new ArgumentException($"Incorrect password for account: {request.Email}.");
                response.Token = _loginToken.GenerateToken(id, request.Email);
                response.Status = true;
            }
            return response;
        }

        private bool TryGetAccountInfoByEmail(string email, out string id, out Secret secret)
        {
            var accountInfo = _loginDbContext.Account.Where(x => x.NormalizedEmail == email.ToUpper())
                .Select(x => new { Id = x.Id, Cipher = x.Cipher, Key = x.Key, IV = x.IV })
                .FirstOrDefault();

            id = accountInfo?.Id.ToString();
            secret = new()
            {
                Cipher = accountInfo?.Cipher,
                Key = accountInfo?.Key,
                IV = accountInfo?.IV
            };

            return accountInfo?.Id is not null && secret.Cipher is not null;
        }

        private async Task<bool> TryLogin(LoginCommand request, Secret secret)
        {
            var recordedPassword = await _cryptoService.DecryptAes(secret);
            return recordedPassword == request.Password;
        }
    }
}
