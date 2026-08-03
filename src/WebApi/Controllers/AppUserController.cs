using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Commands.ExternalUser;
using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Services.Comandos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly ICommandBus commandBus;

        public string ExceptionMessage { get; private set; }

        public AppUserController(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        // GET: api/users
        [HttpGet]
        public IResponse GetUsuarios([FromQuery] ListUsers ownerParameter)
        {
            var respuesta = commandBus.execute(ownerParameter);

            return respuesta;
        }

        // POST: api/Usuarios
        [HttpPost]
        public IResponse Post([FromBody] UserDto userDto)
        {

            return commandBus.execute(new RegisterUser { AppUser = userDto });
        }

        [HttpGet("{id}", Name = "ConsultarUsuarioPorId")]
        public IResponse Get(int id)
        {
            var respuesta = commandBus.execute(new GetUser { Id = id });
            return respuesta;
        }

        [HttpPost]
        [Route("login")]
        public IResponse iniciarSesion([FromBody] SignIn crenciales)
        {
            var respuesta = commandBus.execute(crenciales);
            return respuesta;
        }

        /// <summary>Perfil del usuario autenticado (Tenants + identidad).</summary>
        [HttpGet]
        [Route("me")]
        public IResponse Me()
        {
            return commandBus.execute(new GetCurrentUser());
        }

        [HttpPut]
        [Route("me/profile")]
        public IResponse UpdateMyProfile([FromBody] UpdateCurrentUserProfile body)
        {
            return commandBus.execute(body);
        }

        [HttpPost]
        [Route("close-session")]
        public IResponse SignOut([FromBody] SignOut cerrarSesion)
        {
            var respuesta = commandBus.execute(cerrarSesion);
            return respuesta;
        }

        [HttpPost]
        [Route("change-password")]
        public IResponse MustChangePassword([FromBody] ChangePassword crenciales)
        {
            var respuesta = commandBus.execute(crenciales);
            return respuesta;
        }

        [HttpPost]
        [Route("reset-external-user-password")]
        public IResponse RestablecerContrasena([FromBody] ResetExternalUserPassword datosRestablecer)
        {
            var respuesta = commandBus.execute(datosRestablecer);
            return respuesta;
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] UserDto value)
        {
            commandBus.execute(new EditUser { AppUser = value });
        }

        [HttpPost]
        [Route("temporary-code")]
        public IResponse CodigoTemportal([FromBody] TemporaryCode codigoTemporal)
        {
            var respuesta = commandBus.execute(codigoTemporal);
            return respuesta;
        }

        [HttpPost]
        [Route("codigo")]
        public IResponse Code([FromBody] GetUserByCode codigoTemporal)
        {
            var respuesta = commandBus.execute(codigoTemporal);
            return respuesta;
        }

        [HttpPost]
        [Route("edit-password")]
        public IResponse EditarContraseña([FromBody] EditPassword editarContrasena)
        {
            var respuesta = commandBus.execute(editarContrasena);
            return respuesta;
        }


        [HttpGet("GetSinPermiso/{id}", Name = "ConsultarUsuarioSinPerisoPorId")]
        public IResponse GetSinPermiso(int id)
        {
            var respuesta = commandBus.execute(new ListUsersWithoutPermission { Id = id });

            return respuesta;
        }
    }
}
