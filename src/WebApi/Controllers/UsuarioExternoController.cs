using Aplicacion.Commands.Importador;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Importador;
using Aplicacion.Services.Comandos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioExternoController : ControllerBase
    {
        public ICommandBus CommandBus { get; private set; }

        public UsuarioExternoController(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

        [HttpPost("registro", Name = "registroUsuarioExterno")]
        public IResponse Registro([FromBody] RegistrarUsuarioExterno value)
        {
            return CommandBus.execute(value);
        }

        [HttpPut("verificar-correo", Name = "verificarCorreoUsuarioExterno")]
        public IResponse VerificarCorreo([FromBody] VerificarCorreo verificarCorreo)
        {
            return CommandBus.execute(verificarCorreo);
        }

        [HttpGet("lista", Name = "consultaUsuariosExternos")]
        public IResponse GetLista([FromQuery] ConsultarImportadores ownerParameter)
        {
            return CommandBus.execute(ownerParameter);
        }

        [HttpGet("{id}", Name = "consultaUsuarioExterno")]
        public IResponse GetById(int id)
        {
            return CommandBus.execute(new ConsultarImportador { IdImportador = id });
        }

        [HttpGet("identificador/{id}", Name = "consultaUsuarioExternoPorIdentificador")]
        public IResponse GetPorIdentificador(string id)
        {
            return CommandBus.execute(new ConsultarImportadorPorIdentificador { IdImportador = id });
        }

        [HttpGet("usuario/{id}", Name = "consultaUsuarioExternoPorIdUsuario")]
        public IResponse GetPorUsuario(int id)
        {
            return CommandBus.execute(new ConsultarImportadorPorIdUsuario { IdUsuario = id });
        }

        [HttpPost("invitar", Name = "invitarUsuarioExterno")]
        public void Invitar([FromBody] InvitarImportador value)
        {
            CommandBus.execute(value);
        }

        [HttpPost("rechazar", Name = "rechazarUsuarioExterno")]
        public void Rechazar([FromBody] RechazarSolicitudAcceso value)
        {
            CommandBus.execute(value);
        }

        [HttpPost("solicitar-acceso", Name = "solicitudAccesoUsuarioExterno")]
        public void SolicitudAcceso([FromBody] DtoImportador value)
        {
            CommandBus.execute(new SolicitarAcceso { Importador = value });
        }

        [HttpPost("gestionar-accesos", Name = "gestionarAccesosUsuarioExterno")]
        public void GestionarAccesos([FromBody] GestionarAcceso value)
        {
            CommandBus.execute(value);
        }

        [HttpPost("actualizar", Name = "editarUsuarioExterno")]
        public IResponse Put([FromBody] DtoImportador value)
        {
            return CommandBus.execute(new EditarImportador { Importador = value });
        }

        [HttpPost]
        public void Post([FromBody] DtoImportador importador)
        {
            CommandBus.execute(new CrearImportador { Importador = importador });
        }
    }
}
