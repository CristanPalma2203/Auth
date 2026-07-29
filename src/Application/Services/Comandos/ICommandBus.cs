using Application.Dtos;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Comandos
{
    public interface ICommandBus
    {
        IResponse execute(IMessage comando);
    }
}
