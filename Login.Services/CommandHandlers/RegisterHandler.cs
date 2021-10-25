using System;
using System.Threading;
using System.Threading.Tasks;
using Login.Data.Interface;
using Login.Integration.Interface.Commands;
using Login.Integration.Interface.Responses;

namespace Login.Services.CommandHandlers
{
    public class RegisterHandler : Core.RequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly ILoginDbContext _loginDbContext;

        public RegisterHandler(ILoginDbContext loginDbContext)
        {
            _loginDbContext = loginDbContext;
        }

        protected override Task<RegisterResponse> HandleRequest(RegisterCommand request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
