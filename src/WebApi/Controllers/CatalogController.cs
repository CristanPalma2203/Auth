using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Commands;
using Application.Dtos;
using Application.Services.Comandos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        public ICommandBus CommandBus { get; }

        public CatalogController(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

           
        // GET: api/Catalog/5
        [HttpGet("{tipo}", Name = "ConsultaCatalogo")]
        public IResponse Get(string tipo)
        {
            return CommandBus.execute(new GetCatalog { Type = tipo, ParentId = 0 });
        }

        // GET: api/Catalog/5
        [HttpGet("{tipo}/id-padre/{idpadre}", Name = "ConsultaCatalogoPorPadre")]
        public IResponse Get(string tipo, int idpadre)
        {
            return CommandBus.execute(new GetCatalog { Type = tipo, ParentId = idpadre });
        }
    }
}
