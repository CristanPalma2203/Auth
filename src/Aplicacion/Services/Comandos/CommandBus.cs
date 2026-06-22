using Aplicacion.Common;
using Aplicacion.Dtos;
using Dominio.Service;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Services.Comandos
{
    public class CommandBus : ICommandBus
    {
        private readonly IMediator mediator;

        public CommandBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public IResponse execute(IMessage comando)
        {
            return mediator.Send((IAppMessage)comando).GetAwaiter().GetResult();
        }
    }
}
