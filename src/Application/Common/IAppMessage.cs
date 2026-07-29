using Application.Dtos;
using Domain.Service;
using MediatR;

namespace Application.Common
{
    public interface IAppMessage : IMessage, IRequest<IResponse>
    {
    }
}
