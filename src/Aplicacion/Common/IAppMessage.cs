using Aplicacion.Dtos;
using Dominio.Service;
using MediatR;

namespace Aplicacion.Common
{
    public interface IAppMessage : IMessage, IRequest<IResponse>
    {
    }
}
