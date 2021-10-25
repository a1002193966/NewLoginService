using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Login.Services.Core
{
    public abstract class RequestHandler<TRequest, TResponse> :
        IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected RequestHandler() { }

        protected abstract Task<TResponse> HandleRequest(TRequest request, CancellationToken ct);

        public Task<TResponse> Handle(TRequest request, CancellationToken ct) =>
            HandleRequest(request, ct);     
    }
}
