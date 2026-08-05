using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Dtos.ExternalUser;
using Application.Services.Comandos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalUserController : ControllerBase
    {
        public ICommandBus CommandBus { get; private set; }

        public ExternalUserController(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

        [HttpPost("registro", Name = "registroUsuarioExterno")]
        public IResponse Registro([FromBody] RegisterExternalUser value)
        {
            return CommandBus.execute(value);
        }

        [HttpPut("verificar-correo", Name = "verificarCorreoUsuarioExterno")]
        public IResponse VerifyEmail([FromBody] VerifyEmail verificarCorreo)
        {
            return CommandBus.execute(verificarCorreo);
        }

        [HttpGet("lista", Name = "consultaUsuariosExternos")]
        [HttpGet("list")]
        public IResponse GetLista([FromQuery] ListExternalUsers ownerParameter)
        {
            return CommandBus.execute(ownerParameter);
        }

        [HttpGet("{id}", Name = "consultaUsuarioExterno")]
        public IResponse GetById(int id)
        {
            return CommandBus.execute(new GetExternalUser { IdImportador = id });
        }

        [HttpGet("identifier/{id}", Name = "consultaUsuarioExternoPorIdentificador")]
        public IResponse GetPorIdentificador(string id)
        {
            return CommandBus.execute(new GetExternalUserByIdentifier { IdImportador = id });
        }

        [HttpGet("users/{id}", Name = "getExternalUserByUserId")]
        public IResponse GetPorUsuario(int id)
        {
            return CommandBus.execute(new GetExternalUserByUserId { IdUsuario = id });
        }

        [HttpPost("invitar", Name = "invitarUsuarioExterno")]
        public void Invitar([FromBody] InviteExternalUser value)
        {
            CommandBus.execute(value);
        }

        [HttpPost("rechazar", Name = "rechazarUsuarioExterno")]
        public void Rechazar([FromBody] RejectAccessRequest value)
        {
            CommandBus.execute(value);
        }

        [HttpPost("solicitar-acceso", Name = "solicitudAccesoUsuarioExterno")]
        public void SolicitudAcceso([FromBody] ExternalUserDto value)
        {
            CommandBus.execute(new RequestAccess { ExternalUser = value });
        }

        [HttpPost("gestionar-accesos", Name = "gestionarAccesosUsuarioExterno")]
        public void GestionarAccesos([FromBody] ManageAccess value)
        {
            CommandBus.execute(value);
        }

        [HttpPost("Update", Name = "editarUsuarioExterno")]
        public IResponse Put([FromBody] ExternalUserDto value)
        {
            return CommandBus.execute(new EditExternalUser { ExternalUser = value });
        }

        [HttpPost]
        public void Post([FromBody] ExternalUserDto externalUser)
        {
            CommandBus.execute(new CreateExternalUser { ExternalUser = externalUser });
        }
    }
}
