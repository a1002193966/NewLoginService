using Login.Integration.Interface.Responses;
using MediatR;

namespace Login.Integration.Interface.Commands
{
    public class LoginCommand :  IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
