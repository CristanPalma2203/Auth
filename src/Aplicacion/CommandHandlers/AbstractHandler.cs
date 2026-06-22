using Aplicacion.Common;
using Aplicacion.Dtos;
using Dominio.Service;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.CommandHandlers
{
    public abstract class AbstractHandler<T> : IRequestHandler<T, IResponse> where T : class, IAppMessage
    {
        public abstract IResponse Handle(T message);

        public Task<IResponse> Handle(T request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Handle(request));
        }

        public IResponse ejecutar(IMessage message)
        {
            return Handle((T)message);
        }
    }
}
