using Aplicacion.Commands.Importador;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Importador;
using MapsterMapper;
using Dominio.Especificaciones;
using Dominio.Repositories;
using Dominio.Repositories.Extensiones;

namespace Aplicacion.CommandHandlers.Importador
{
    public class ConsultarImportadoresHandler : AbstractHandler<ConsultarImportadores>
    {
        private readonly IUsuarioExternoRepository importadorRepository;
        private readonly IMapper mapper;

        public ConsultarImportadoresHandler(IUsuarioExternoRepository importadorRepository, IMapper mapper) {
            this.importadorRepository = importadorRepository;
            this.mapper = mapper;
        }
        public override IResponse Handle(ConsultarImportadores message)
        {
            IPagina<Dominio.Models.UsuarioExterno> respuesta;
            if (!string.IsNullOrWhiteSpace(message.identificador ) || !string.IsNullOrWhiteSpace(message.nombre) )
            {
                respuesta = importadorRepository.ConsultarPaginado(message, new BuscarImportadorPorIdentificadorYNombre(message.identificador, message.nombre));
            }
            else
            {
                var consulta = !string.IsNullOrWhiteSpace(message.Consulta) ? message.Consulta : "FechaAprobacionAcceso = null AND CorreoVerificado=true"; //"FechaAprobacionAcceso = null AND FechaDenegacionAcceso = null AND CorreoVerificado=true";
                respuesta = importadorRepository.Filter(message, consulta);
            }
           
           return mapper.Map<DtoImportadoresPaginados>(respuesta); 
        }
    }
}
