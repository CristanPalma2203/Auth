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
    public class PermissionController : ControllerBase
    {
        public PermissionController(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

        public ICommandBus CommandBus { get; }

        // GET: api/Permission
        [HttpGet]
        public IResponse Get()
        {
            return CommandBus.execute(new ListPermissions());
        }
    }
}
