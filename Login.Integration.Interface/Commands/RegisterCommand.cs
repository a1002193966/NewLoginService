using Login.Integration.Interface.Responses;
using MediatR;

namespace Login.Integration.Interface.Commands
{
    public class RegisterCommand : IRequest<RegisterResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
