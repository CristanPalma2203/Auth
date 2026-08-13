using Application.Commands;
using Application.Dtos;
using Application.Services.Comandos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CatalogController : ControllerBase
    {
        public ICommandBus CommandBus { get; }

        public CatalogController(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

        [HttpGet("{tipo}", Name = "ConsultaCatalogo")]
        public IResponse Get(string tipo)
        {
            return CommandBus.execute(new GetCatalog { Type = tipo, ParentId = 0 });
        }

        [HttpGet("{tipo}/id-padre/{idpadre}", Name = "ConsultaCatalogoPorPadre")]
        public IResponse Get(string tipo, int idpadre)
        {
            return CommandBus.execute(new GetCatalog { Type = tipo, ParentId = idpadre });
        }
    }
}
