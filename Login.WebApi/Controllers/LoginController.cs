using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Login.Integration.Interface.Commands;
using Login.Integration.Interface.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Login.WebApi.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<RegisterResponse>> RegisterAsync(
            [FromBody] RegisterCommand request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return Created("/register", response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LoginResponse>> LoginAsync(
            [FromBody] LoginCommand request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return Ok(response);
        }
    }
}
