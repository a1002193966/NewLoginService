using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Data.Interface;
using Login.DomainModel;
using Login.Integration.Interface.Commands;
using Login.Integration.Interface.Responses;
using Login.Services.UtilityServices.PasswordService;
using Microsoft.Extensions.Configuration;

namespace Login.Services.CommandHandlers
{
    public class RegisterHandler : Core.RequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly ILoginDbContext _loginDbContext;
        private readonly ICryptoService _cryptoService;
        private readonly string _connectionString;

        public RegisterHandler(ILoginDbContext loginDbContext, ICryptoService cryptoService, IConfiguration configuration)
        {
            _loginDbContext = loginDbContext;
            _cryptoService = cryptoService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected override async Task<RegisterResponse> HandleRequest(RegisterCommand request, CancellationToken ct)
        {
            if (await IsExistAsync(request.Email))
                throw new ArgumentException($"Email: {request.Email} is already registered.");

            var account = await CreateNewAccount(request);


            await _loginDbContext.Account.AddAsync(account, ct);
            await _loginDbContext.SaveChangesAsync(ct);

            RegisterResponse response = new()
            {
                Id = account.Id,
                Status = true
            };
            return response;
        }

        private async Task<bool> IsExistAsync(string email)
        {
            return _loginDbContext
                .Account
                .Any(x =>
                     x.NormalizedEmail == email.ToUpper() &&
                     !x.IsDeleted);
        }

        private async Task<Account> CreateNewAccount(RegisterCommand request)
        {
            var secret = await _cryptoService.EncryptAes(request.Password);
            return new()
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
                IsDeleted = false,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                Cipher = secret.Cipher,
                Key = secret.Key,
                IV = secret.IV
            };
        }


    }
}
