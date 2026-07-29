
using Application.Commands.Role;
using Application.Dtos;
using Application.Services.Comandos;
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly ICommandBus commandBus;
        private readonly IRoleRepository roleRepository;

        public RoleController(ICommandBus commandBus, IRoleRepository roleRepository)
        {
            this.commandBus = commandBus;
            this.roleRepository = roleRepository;
        }


        /// <summary>
        ///  Consulta todos los roles y los devuelve paginados.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /roles
        ///
        /// </remarks>
        /// <param name="ownerParameter"></param>
        /// <returns>Lista de roles</returns>
        /// <response code="200">Lista de roles paginados</response>
        /// <response code="500">Error interno</response> 
        /// <response code="401">NO Autorizado</response>
        /// <response code="403">NO tiene Permissions para ejecutar el metodo</response>
        // GET: api/roles
        [HttpGet]
        public IResponse GetRoles([FromQuery] ListRoles ownerParameter)
        {
            var respuesta = commandBus.execute(ownerParameter);


            return respuesta;
        }
        /// <summary>
        /// Consulta todos los roles y los devuelve sin paginar.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Role/sinpaginar
        ///
        /// </remarks>
        /// <returns>Lista de roles</returns>
        /// <response code="200">Lista de roles sin paginar</response>
        /// <response code="500">Error interno</response> 
        /// <response code="401">NO Autorizado</response>
        /// <response code="403">NO tiene Permissions para ejecutar el metodo</response>
        [HttpGet]
        [Route("unpaged")]
        public IResponse ConsultarSinPaginar([FromQuery] ListRolesUnpaged consulta)
        {
            var respuesta = commandBus.execute(consulta);
            return respuesta;
        }

        /// <summary>
        /// Consulta un Roles con sus permisos.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Role/1
        ///
        /// </remarks>
        /// <returns>Role consultado</returns>
        /// <param name="id"></param>
        /// <response code="200"></response>
        /// <response code="500">Error interno</response> 
        /// <response code="401">NO Autorizado</response>
        /// <response code="403">NO tiene Permissions para ejecutar el metodo</response>
        [HttpGet("{id}", Name = "GetById")]
        public IResponse Get(int id)
        {
            var respuesta = commandBus.execute(new GetRole { id = id });
            return respuesta;
        }

        /// <summary>
        /// Crea un nuevo Role.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Role
        ///     {
        ///     "nombre":"Role de prueba",
        ///     "descripcion":
        ///     "Roles de prueba",
        ///     "permisos":[1,8,12]
        ///     }
        ///
        /// </remarks>
        /// <param name="Roles"></param>
        /// <returns>Nuevo Roles creado</returns>
        /// <response code="200">Role Creado Satisfactoriamente</response>
        /// <response code="500">Error interno</response> 
        /// <response code="401">NO Autorizado</response>
        /// <response code="403">NO tiene Permissions para ejecutar el metodo</response>
        // POST: api/Role
        [HttpPost]
        public void Post([FromBody] RoleDto Roles)
        {
            commandBus.execute(new CreateRole { Role = Roles });
        }

        /// <summary>
        /// Actualiza un Roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Role
        ///       {
        ///       "id":1,
        ///       "nombre":"Prueba",
        ///       "descripcion":"Prueba 2",
        ///       "permisos":[1,2,8,10]
        ///       }
        ///
        /// </remarks>
        /// <returns>Role actualizadoo</returns>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <response code="200"></response>
        /// <response code="500">Error interno</response> 
        /// <response code="401">NO Autorizado</response>
        /// <response code="403">NO tiene Permissions para ejecutar el metodo</response>
        // PUT: api/roles/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] RoleDto value)
        {
            commandBus.execute(new EditRole { Role = value, Id = id });
        }

        [HttpGet("modules", Name = "modulos-roles")]
        public List<Role> ModulosRoles()
        {
            return roleRepository.Filter(new RoleModules()).ToList(); 
        }


    }
}
